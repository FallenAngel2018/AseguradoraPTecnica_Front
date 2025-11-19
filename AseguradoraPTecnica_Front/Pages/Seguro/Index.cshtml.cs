using AseguradoraPTecnica_Front.Models.Seguro;
using AseguradoraPTecnica_Front.Services;
using AseguradoraPTecnica_Front.Services.Seguro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AseguradoraPTecnica_Front.Pages.Seguro
{
    public class SeguroIndexModel : PageModel
    {
        private readonly ISeguroApiService _seguroService;

        public SeguroIndexModel(ISeguroApiService seguroService)
        {
            _seguroService = seguroService;
        }

        public List<SeguroViewModel> Seguros { get; set; }
        public string MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _seguroService.ObtenerTodosAsync();

                if (response.success)
                {
                    Seguros = response.data;
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

        [BindProperty]
        public SeguroInputModel NuevoSeguro { get; set; }
        public async Task<IActionResult> OnPostAgregarSeguroAsync()
        {
            if (!ModelState.IsValid)
            {
                // Retornar página con errores para mostrar mensajes o modal volver a abrir
                return Page();
            }

            // Aquí usas NuevoSeguro que estará lleno con los datos del formulario
            // Ejemplo: guardarlo en base de datos vía servicio
            await _seguroService.CrearSeguroAsync(NuevoSeguro);

            // Rediriges o retornas JSON, según necesidad
            return RedirectToPage(); // O usar Partial, JsonResult, etc.
        }


        //public void OnGet()
        //{
        //    Seguros = new List<SeguroViewModel>
        //    {
        //        new SeguroViewModel
        //        {
        //            IdSeguro = 1,
        //            NombreSeguro = "Seguro de Auto",
        //            Codigo = "SA001",
        //            SumaAsegurada = 10000,
        //            Prima = 1500,
        //            EdadMinima = 31,
        //            EdadMaxima = 119,
        //            Estado = 1
        //        },
        //        new SeguroViewModel
        //        {
        //            IdSeguro = 2,
        //            NombreSeguro = "Seguro de Salud",
        //            Codigo = "SS001",
        //            SumaAsegurada = 7500,
        //            Prima = 1500,
        //            EdadMinima = 31,
        //            EdadMaxima = 119,
        //            Estado = 1
        //        },
        //        new SeguroViewModel
        //        {
        //            IdSeguro = 3,
        //            NombreSeguro = "Seguro de Vida",
        //            Codigo = "SV003",
        //            SumaAsegurada = 30000,
        //            Prima = 800,
        //            EdadMinima = 0,
        //            EdadMaxima = 19,
        //            Estado = 1
        //        }
        //    };
        //}



    }
}
