using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StudyAndChill.API.Dtos.External.Asaas;
using StudyAndChill.API.Models;

namespace StudyAndChill.API.Services
{
    public class AsaasService : IAsaasService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AsaasService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var apikey = _configuration["Asaas:ApiKey"] ?? _configuration["ASAAS_API_KEY"];
            var baseUrl = _configuration["Asaas:Url"] ?? _configuration["ASAAS_URL"];
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("access_token", apikey);

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "StudyAndChill-App/1.0");
        }

        public async Task<string?> CreateCustomer(User user, StudentProfile profile)
        {
            var request = new AsaasCustomerRequest
            {
                Name = user.Name,
                Email = user.Email,
                CpfCnpj = profile.DocumentNumber,
                MobilePhone = profile.Phone,
                PostalCode = profile.Address.PostalCode,
                Address = profile.Address.Street,
                AddressNumber = profile.Address.Number,
                Complement = profile.Address.Complement,
                Province = profile.Address.Neighborhood,
                State = profile.Address.State,
                City = profile.Address.City,
                ExternalReference = user.Id.ToString()
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await _httpClient.PostAsync("customers", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var customer = JsonSerializer.Deserialize<AsaasCustomerResponse>(content);
                    return customer?.Id;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro Asaas: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro de conexão Asaas: {ex.Message}");
            }
            return null;
        }
    }
}