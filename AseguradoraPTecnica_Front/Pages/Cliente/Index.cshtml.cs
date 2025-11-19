using AseguradoraPTecnica_Front.Models;
using AseguradoraPTecnica_Front.Services;
using AseguradoraPTecnica_Front.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Text.Json;

namespace AseguradoraPTecnica_Front.Pages.Cliente
{
    public class IndexModel : PageModel
    {
        private readonly IClienteApiService _clienteService;

        public IndexModel(IClienteApiService clienteService)
        {
            _clienteService = clienteService;
        }

        public List<ClienteViewModel> Clientes { get; set; } = new List<ClienteViewModel>();
        public string MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _clienteService.ObtenerTodosAsync();

                if (response.success)
                {
                    Clientes = response.data;
                }
                else
                {
                    MensajeError = response.message;
                }

                return Page();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error de conexión: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCargarArchivoAsync(IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                {
                    return new JsonResult(new { success = false, message = "No se recibió archivo" });
                }

                var extension = Path.GetExtension(archivo.FileName).ToLower();

                if (extension != ".xlsx" && extension != ".txt")
                {
                    return new JsonResult(new { success = false, message = "Solo .xlsx o .txt" });
                }

                if (archivo.Length > 5 * 1024 * 1024)
                {
                    return new JsonResult(new { success = false, message = "Máximo 5MB" });
                }

                byte[] archivoBytes;
                using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    archivoBytes = ms.ToArray();
                }

                using (var streamValidacion = new MemoryStream(archivoBytes))
                {
                    var validacion = extension == ".txt"
                        ? await FileUtils.ValidarArchivoTxtAsync(streamValidacion)
                        : FileUtils.ValidarArchivoXlsxAsync(streamValidacion);

                    if (!validacion.esValido)
                    {
                        return new JsonResult(new { success = false, message = validacion.mensaje });
                    }
                }

                using (var streamEnvio = new MemoryStream(archivoBytes))
                {
                    var archivoTemp = new FormFile(streamEnvio, 0, archivoBytes.Length, "archivo", archivo.FileName)
                    {
                        Headers = archivo.Headers,
                        ContentType = archivo.ContentType
                    };

                    var resultado = await _clienteService.EnviarArchivoAApiAsync(archivoTemp);

                    return new JsonResult(resultado);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }



        private async Task<(bool esValido, string mensaje)> ValidarContenidoArchivoAsync(Stream archivo, string extension)
        {
            try
            {
                if (extension == ".txt")
                {
                    return await FileUtils.ValidarArchivoTxtAsync(archivo);
                }
                else if (extension == ".xlsx")
                {
                    return FileUtils.ValidarArchivoXlsxAsync(archivo);
                }

                return (false, "Extensión no soportada");
            }
            catch (Exception ex)
            {
                return (false, $"Error al validar el archivo: {ex.Message}");
            }
        }

        

        



    }

}
