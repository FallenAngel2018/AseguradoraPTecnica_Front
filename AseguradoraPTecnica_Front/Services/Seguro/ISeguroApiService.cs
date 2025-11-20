using AseguradoraPTecnica_Front.Models;
using AseguradoraPTecnica_Front.Models.Seguro;

namespace AseguradoraPTecnica_Front.Services.Seguro
{
    public interface ISeguroApiService
    {
        Task<ApiResponse<List<SeguroViewModel>>> ObtenerTodosAsync();
        Task<ApiResponse<SeguroViewModel>> CrearSeguroAsync(SeguroInputModel nuevoSeguro);
        Task<ApiResponse<List<SegurosContratadosViewModel>>> ObtenerTodosLosSegurosContratadosAsync();
        Task<ApiResponse<SeguroAsignadoDetalleViewModel>> AsignarSegurosAsync(SeguroAsignadoInputModel seguros);
        Task<ApiResponse<List<SegurosContratadosViewModel>>> BuscarSegurosContratadosPorCedulaOCodSeguroAsync(string arg_busqueda, int opcion);

    }
}
