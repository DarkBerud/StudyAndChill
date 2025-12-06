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
            var cpfCnpj = RemovePunctuation(profile.DocumentNumber);
            try
            {
                var getResponse = await _httpClient.GetAsync($"customers?cpfCnpj={cpfCnpj}");
                if (getResponse.IsSuccessStatusCode)
                {
                    var content = await getResponse.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<AsaasCustomerListResponse>(content);

                    if (list != null && list.Data.Any())
                    {
                        return list.Data.First().Id;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Erro ao tentar buscar o cliente existente no Asaas");
            }

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
                return null;
            }
        }

        public async Task<string?> CreateSubscription(string customerId, decimal value, DateOnly nextDueDate, DateOnly? endDate = null)
        {
            var request = new AsaasSubscriptionRequest
            {
                Customer = customerId,
                Value = value,
                NextDueDate = nextDueDate.ToString("yyyy-MM-dd"),
                Description = "Mensalidade - Curso de Inglês",
                EndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await _httpClient.PostAsync("subscriptions", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var subscripition = JsonSerializer.Deserialize<AsaasSubscriptionResponse>(content);
                    return subscripition?.Id;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error Asaas Subscription ({response.StatusCode}): {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro conexão Asaas Subscription: {ex.Message}");
                return null;
            }
        }

        private string RemovePunctuation(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return new string(input.Where(char.IsDigit).ToArray());
        }
    }
}