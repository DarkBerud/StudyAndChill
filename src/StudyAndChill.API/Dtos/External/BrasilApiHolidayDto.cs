using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos.External
{
    public class BrasilApiHolidayDto
    {
        [JsonPropertyName("date")]
        public DateTime Date {get; set; }
        
        [JsonPropertyName("name")]
        public string Name {get; set; }

        [JsonPropertyName("type")]
        public string Type {get; set; }
    }
}