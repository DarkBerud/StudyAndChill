using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.External.Asaas
{
    public class AsaasWebhookEventDto
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("payment")]
        public AsaasWebhookPaymentDto Payment { get; set; }
    }
}