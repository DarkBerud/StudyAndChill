using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class CreateAvailabilityDto
    {
        [Required]
        public int DayOfWeek { get; set; }

        [Required]
        public TimeOnly AvailableFrom { get; set; }

        [Required]
        public TimeOnly AvailableTo { get; set; }
    }
}