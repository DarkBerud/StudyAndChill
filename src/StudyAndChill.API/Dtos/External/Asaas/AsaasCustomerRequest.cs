using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.External.Asaas
{
    public class AsaasCustomerRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cpfCnpj")]
        public string CpfCnpj { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("AddressNumber")]
        public string AddressNumber { get; set; }

        [JsonPropertyName("complement")]
        public string? Complement { get; set; }

        [JsonPropertyName("province")]
        public string Province { get; set; }

         [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }


        [JsonPropertyName("externalReference")]
        public string ExternalReference { get; set; }
    }
}