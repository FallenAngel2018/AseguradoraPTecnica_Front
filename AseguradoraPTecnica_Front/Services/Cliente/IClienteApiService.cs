using AseguradoraPTecnica_Front.Models;
using AseguradoraPTecnica_Front.Models.Cliente;

namespace AseguradoraPTecnica_Front.Services.Cliente
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
