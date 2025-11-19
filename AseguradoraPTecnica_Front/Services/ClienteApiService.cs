using System.Text;
using System.Text.Json;
using AseguradoraPTecnica_Front.Models;

namespace AseguradoraPTecnica_Front.Services
{
    public class ClienteApiService : IClienteApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ClienteApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<ClienteApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // GET: Obtener todos los clientes
        public async Task<ApiResponse<List<ClienteViewModel>>> ObtenerTodosAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");
                var response = await httpClient.GetAsync("Cliente/GetAll");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ClienteViewModel>>>(content, _jsonOptions);
                    return apiResponse ?? new ApiResponse<List<ClienteViewModel>>
                    {
                        success = false,
                        message = "Respuesta vacía de la API",
                        data = new List<ClienteViewModel>()
                    };
                }

                return new ApiResponse<List<ClienteViewModel>>
                {
                    success = false,
                    message = $"Error: {response.StatusCode}",
                    data = new List<ClienteViewModel>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ClienteViewModel>>
                {
                    success = false,
                    message = ex.Message,
                    data = new List<ClienteViewModel>()
                };
            }
        }

        // GET: Obtener cliente por ID
        public async Task<ApiResponse<ClienteViewModel>> ObtenerPorIdAsync(long id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");
                var response = await httpClient.GetAsync($"cliente/{id}");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteViewModel>>(content, _jsonOptions);
                    return apiResponse ?? new ApiResponse<ClienteViewModel>
                    {
                        success = false,
                        message = "Cliente no encontrado",
                        data = null
                    };
                }

                return new ApiResponse<ClienteViewModel>
                {
                    success = false,
                    message = $"Error: {response.StatusCode}",
                    data = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClienteViewModel>
                {
                    success = false,
                    message = ex.Message,
                    data = null
                };
            }
        }

        // POST: Crear nuevo cliente
        public async Task<ApiResponse<ClienteViewModel>> CrearAsync(ClienteViewModel cliente)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");

                var json = JsonSerializer.Serialize(cliente, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("cliente", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteViewModel>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<ClienteViewModel>
                    {
                        success = true,
                        message = "Cliente creado exitosamente",
                        data = cliente
                    };
                }

                // Intentar parsear el error
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<ClienteViewModel>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<ClienteViewModel>
                    {
                        success = false,
                        message = "Error al crear cliente",
                        data = null
                    };
                }
                catch
                {
                    return new ApiResponse<ClienteViewModel>
                    {
                        success = false,
                        message = $"Error al crear cliente: {response.StatusCode}",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClienteViewModel>
                {
                    success = false,
                    message = ex.Message,
                    data = null
                };
            }
        }

        // PUT: Actualizar cliente
        public async Task<ApiResponse<ClienteViewModel>> ActualizarAsync(long id, ClienteViewModel cliente)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");

                var json = JsonSerializer.Serialize(cliente, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"cliente/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ClienteViewModel>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<ClienteViewModel>
                    {
                        success = true,
                        message = "Cliente actualizado exitosamente",
                        data = cliente
                    };
                }

                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<ClienteViewModel>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<ClienteViewModel>
                    {
                        success = false,
                        message = "Error al actualizar cliente",
                        data = null
                    };
                }
                catch
                {
                    return new ApiResponse<ClienteViewModel>
                    {
                        success = false,
                        message = $"Error al actualizar cliente: {response.StatusCode}",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClienteViewModel>
                {
                    success = false,
                    message = ex.Message,
                    data = null
                };
            }
        }

        // DELETE: Eliminar cliente
        public async Task<ApiResponse<object>> EliminarAsync(long id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");
                var response = await httpClient.DeleteAsync($"cliente/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<object>
                    {
                        success = true,
                        message = "Cliente eliminado exitosamente",
                        data = null
                    };
                }

                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<object>
                    {
                        success = false,
                        message = "Error al eliminar cliente",
                        data = null
                    };
                }
                catch
                {
                    return new ApiResponse<object>
                    {
                        success = false,
                        message = $"Error al eliminar cliente: {response.StatusCode}",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    success = false,
                    message = ex.Message,
                    data = null
                };
            }
        }

        public async Task<object> EnviarArchivoAApiAsync(IFormFile archivo)
        {
            var client = _httpClientFactory.CreateClient("SegurosAPI");

            using (var content = new System.Net.Http.MultipartFormDataContent())
            {
                var fileContent = new StreamContent(archivo.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivo.ContentType);
                content.Add(fileContent, "archivo", archivo.FileName);

                try
                {
                    var response = await client.PostAsync("Cliente/cargar-archivo-ingreso-clientes", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = JsonSerializer.Deserialize<ApiResponse<object>>(
                            responseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        return new
                        {
                            success = resultado?.success ?? false,
                            message = resultado?.message ?? "Archivo procesado correctamente",
                            data = resultado?.data ?? ""
                        };
                    }
                    else
                    {
                        return new { success = false, message = $"Error en la API: {response.StatusCode}" };
                    }
                }
                catch (Exception)
                {
                    return new { success = false, message = "Error de conexión con la API" };
                }
            }
        }

    }
}
