using AseguradoraPTecnica_Front.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using ClosedXML.Excel;
using System.Text.Json;
using AseguradoraPTecnica_Front.Models.Cliente;
using AseguradoraPTecnica_Front.Services.Cliente;

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
                    var pruba = FileUtils.ValidarArchivoOpenXml(streamValidacion);

                    var validacion = extension == ".txt"
                        ? await FileUtils.ValidarArchivoTxtAsync(streamValidacion)
                        : FileUtils.ValidarArchivoXlsx(archivo);

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

        public async Task<PartialViewResult> OnGetActualizarTablaParcialAsync()
        {
            var clientes = await _clienteService.ObtenerTodosAsync();
            return Partial("_TablaClientes", clientes);
        }








    }

}
