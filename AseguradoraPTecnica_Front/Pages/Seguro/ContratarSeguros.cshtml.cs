using AseguradoraPTecnica_Front.Models;
using AseguradoraPTecnica_Front.Models.Cliente;
using AseguradoraPTecnica_Front.Models.Estado;
using AseguradoraPTecnica_Front.Models.Seguro;
using AseguradoraPTecnica_Front.Services.Cliente;
using AseguradoraPTecnica_Front.Services.Seguro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AseguradoraPTecnica_Front.Pages.Seguro
{
    public class ContratarSegurosModel : PageModel
    {
        private readonly ISeguroApiService _seguroService;
        private readonly IClienteApiService _clienteService;

        public ContratarSegurosModel(ISeguroApiService seguroService, IClienteApiService clienteService)
        {
            _seguroService = seguroService;
            _clienteService = clienteService;
        }

        public List<SegurosContratadosViewModel> SegurosContratados { get; set; }
        public string? MensajeErrorSegurosContratados { get; set; }
        public List<SeguroViewModel> Seguros { get; set; }
        public string? MensajeErrorSeguros { get; set; }
        public List<ClienteViewModel> Clientes { get; set; }
        public string? MensajeErrorClientes { get; set; }
        public List<EstadoViewModel> Estados { get; set; }
        public string? MensajeErrorEstados { get; set; }
        public string? MensajeError { get; set; }



        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await ObtenerTodosLosSegurosContratados();
                await ObtenerTodosLosSeguros();
                await ObtenerTodosLosClientes();
                
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

        public async Task<IActionResult> OnPostAsync()
        {
            var cedulaCliente = Request.Form["CedulaCliente"];
            var codSeguros = Request.Form["CodSeguros"].ToList();
            var estadoStr = Request.Form["Estado"].ToString();
            int estado = int.Parse(estadoStr);

            var segurosAsignados = new SeguroAsignadoInputModel
            {
                Cedula = cedulaCliente,
                CodigosSeguros = codSeguros,
                Estado = estado
            };

            var segurosAsignadosResponse = await _seguroService.AsignarSegurosAsync(segurosAsignados);

            await ObtenerTodosLosSegurosContratados();
            await ObtenerTodosLosSeguros();
            await ObtenerTodosLosClientes();

            return RedirectToPage();
        }


        



        #region Consultas

        private async Task<ApiResponse<List<SegurosContratadosViewModel>>> ObtenerTodosLosSegurosContratados()
        {
            var responseSegurosContratados = await _seguroService.ObtenerTodosLosSegurosContratadosAsync();
            if (responseSegurosContratados.success)
            {
                SegurosContratados = responseSegurosContratados.data;
            }
            else
            {
                MensajeErrorSegurosContratados = responseSegurosContratados.message;
            }

            return responseSegurosContratados;
        }

        private async Task<ApiResponse<List<SeguroViewModel>>> ObtenerTodosLosSeguros()
        {
            var responseSeguros = await _seguroService.ObtenerTodosAsync();
            if (responseSeguros.success)
            {
                Seguros = responseSeguros.data;
            }
            else
            {
                MensajeErrorSeguros = responseSeguros.message;
            }

            return responseSeguros;
        }

        private async Task<ApiResponse<List<ClienteViewModel>>> ObtenerTodosLosClientes()
        {
            var responseClientes = await _clienteService.ObtenerTodosAsync();
            if (responseClientes.success)
            {
                Clientes = responseClientes.data;
            }
            else
            {
                MensajeErrorClientes += " " + responseClientes.message;
            }

            return responseClientes;

        }

        #endregion


    }
}
