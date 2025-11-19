using AseguradoraPTecnica_Front.Models;

namespace AseguradoraPTecnica_Front.Services
{
    public interface IClienteApiService
    {
        Task<ApiResponse<List<ClienteViewModel>>> ObtenerTodosAsync();
        Task<ApiResponse<ClienteViewModel>> ObtenerPorIdAsync(long id);
        Task<ApiResponse<ClienteViewModel>> CrearAsync(ClienteViewModel cliente);
        Task<ApiResponse<ClienteViewModel>> ActualizarAsync(long id, ClienteViewModel cliente);
        Task<ApiResponse<object>> EliminarAsync(long id);
        Task<object> EnviarArchivoAApiAsync(IFormFile archivo);
    }
}
