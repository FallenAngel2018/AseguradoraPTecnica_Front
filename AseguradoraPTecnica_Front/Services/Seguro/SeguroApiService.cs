using AseguradoraPTecnica_Front.Models;
using AseguradoraPTecnica_Front.Models.Cliente;
using AseguradoraPTecnica_Front.Models.Seguro;
using AseguradoraPTecnica_Front.Services.Seguro;
using System.Net.Http;
using System.Text.Json;

namespace AseguradoraPTecnica_Front.Services.Seguro
{
    public class SeguroApiService : ISeguroApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public SeguroApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<SeguroApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<ApiResponse<List<SeguroViewModel>>> ObtenerTodosAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");
                var response = await httpClient.GetAsync("Seguro/GetAll");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<SeguroViewModel>>>(content, _jsonOptions);
                    return apiResponse ?? new ApiResponse<List<SeguroViewModel>>
                    {
                        success = false,
                        message = "Respuesta vacía de la API",
                        data = new List<SeguroViewModel>()
                    };
                }

                return new ApiResponse<List<SeguroViewModel>>
                {
                    success = false,
                    message = $"Error: {response.StatusCode}",
                    data = new List<SeguroViewModel>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SeguroViewModel>>
                {
                    success = false,
                    message = ex.Message,
                    data = new List<SeguroViewModel>()
                };
            }
        }

        public async Task<ApiResponse<SeguroViewModel>> CrearSeguroAsync(SeguroInputModel nuevoSeguro)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");

                // Cambia PostAsync por PostAsJsonAsync para enviar JSON automáticamente
                var response = await httpClient.PostAsJsonAsync("Seguro/NuevoSeguro", nuevoSeguro);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<SeguroViewModel>>();
                    return result;
                }
                else
                {
                    return new ApiResponse<SeguroViewModel>
                    {
                        success = false,
                        message = $"Error: {response.StatusCode}",
                        data = new SeguroViewModel()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<SeguroViewModel>
                {
                    success = false,
                    message = ex.Message,
                    data = new SeguroViewModel()
                };
            }
        }

        
        public async Task<ApiResponse<List<SegurosContratadosViewModel>>> ObtenerTodosLosSegurosContratadosAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");
                var response = await httpClient.GetAsync("Seguro/GetSegurosAsignados");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<SegurosContratadosViewModel>>>(content, _jsonOptions);
                    return apiResponse ?? new ApiResponse<List<SegurosContratadosViewModel>>
                    {
                        success = false,
                        message = "Respuesta vacía de la API",
                        data = new List<SegurosContratadosViewModel>()
                    };
                }

                return new ApiResponse<List<SegurosContratadosViewModel>>
                {
                    success = false,
                    message = $"Error: {response.StatusCode}",
                    data = new List<SegurosContratadosViewModel>()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SegurosContratadosViewModel>>
                {
                    success = false,
                    message = ex.Message,
                    data = new List<SegurosContratadosViewModel>()
                };
            }
        }


        public async Task<ApiResponse<SeguroAsignadoDetalleViewModel>> AsignarSegurosAsync(SeguroAsignadoInputModel seguros)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("SegurosAPI");

                var response = await httpClient.PostAsJsonAsync("Seguro/AsignarSeguros", seguros);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<SeguroAsignadoDetalleViewModel>>();
                    return result;
                }
                else
                {
                    return new ApiResponse<SeguroAsignadoDetalleViewModel>
                    {
                        success = false,
                        message = $"Error: {response.StatusCode}",
                        data = new SeguroAsignadoDetalleViewModel()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<SeguroAsignadoDetalleViewModel>
                {
                    success = false,
                    message = ex.Message,
                    data = new SeguroAsignadoDetalleViewModel()
                };
            }
        }

        public Task<List<SeguroAsignadoDetalleViewModel>> GetAssignedInsurancesDetailsByCedulaAsync(string cedula)
        {
            throw new NotImplementedException();
        }
    }
}
