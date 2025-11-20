using AseguradoraPTecnica_Front.Models.Cliente;
using AseguradoraPTecnica_Front.Models.Estado;
using AseguradoraPTecnica_Front.Models.Seguro;
using AseguradoraPTecnica_Front.Services;
using AseguradoraPTecnica_Front.Services.Cliente;
using AseguradoraPTecnica_Front.Services.Seguro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AseguradoraPTecnica_Front.Pages.Seguro
{
    public class SeguroIndexModel : PageModel
    {
        private readonly ISeguroApiService _seguroService;
        private readonly IClienteApiService _clienteService;

        public SeguroIndexModel(ISeguroApiService seguroService, IClienteApiService clienteService)
        {
            _seguroService = seguroService;
            _clienteService = clienteService;
            
        }

        public List<SeguroViewModel> Seguros { get; set; }
        public List<ClienteViewModel> Clientes { get; set; }
        public List<EstadoViewModel> Estados { get; set; }
        public string MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var responseSeguros = await _seguroService.ObtenerTodosAsync();
                if (responseSeguros.success)
                {
                    Seguros = responseSeguros.data;
                }
                else
                {
                    MensajeError = responseSeguros.message;
                }


                Estados = new List<EstadoViewModel>
                {
                    new EstadoViewModel { IdEstado = 1, Estado = "Activo" },
                    new EstadoViewModel { IdEstado = 2, Estado = "Pendiente" },
                    new EstadoViewModel { IdEstado = 0, Estado = "Inactivo" }
                };

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
                return Page();
            }

            await _seguroService.CrearSeguroAsync(NuevoSeguro);

            return RedirectToPage();
        }

        
    }
}
