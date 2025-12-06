using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.External.Asaas
{
    public class AsaasWebhookPaymentDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("subscription")]
        public string? Subscription { get; set; }

        [JsonPropertyName("customer")]
        public string Customer { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("netValue")]
        public decimal NetValue { get; set; }

        [JsonPropertyName("dateCreated")]
        public string DateCreated { get; set; }

        [JsonPropertyName("paymentDate")]
        public string? PaymentDate { get; set; }

        [JsonPropertyName("billingType")]
        public string BillingType { get; set; }

        [JsonPropertyName("invoiceUrl")]
        public string? InvoiceUrl { get; set; }
    }
}