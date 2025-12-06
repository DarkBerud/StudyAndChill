using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.External.Asaas
{
    public class AsaasCustomerListResponse
    {
        [JsonPropertyName("data")]
        public List<AsaasCustomerResponse> Data { get; set; }
    }
}