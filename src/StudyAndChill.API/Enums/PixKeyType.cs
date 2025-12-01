using System.Text.Json.Serialization;

namespace StudyAndChill.API.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PixKeyType
    {
        CPF,
        CNPJ,
        Email,
        Phone,
        Random
    }
}