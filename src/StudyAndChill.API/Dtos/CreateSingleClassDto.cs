using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class CreateSingleClassDto
    {
        public int StudentId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationInMinutes { get; set; }
    }
}