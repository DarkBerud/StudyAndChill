using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.External.Asaas
{
    public class AsaasSubscriptionRequest
    {
        [JsonPropertyName("customer")]
        public string Customer { get; set; }

        [JsonPropertyName("billingType")]
        public string BillingType { get; set; } = "BOLETO";

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("nextDueDate")]
        public string NextDueDate { get; set; }

        [JsonPropertyName("cycle")]
        public string Cycle { get; set; } = "MONTHLY";

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }
    }
}