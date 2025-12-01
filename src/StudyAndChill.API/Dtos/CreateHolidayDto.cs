using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class CreateHolidayDto
    {
        [Required]
        public string Name {get; set; }
        [Required]
        public DateOnly Date {get; set; }
        public bool IsGlobal {get; set; } = true;

        public List<int>? TargetUserIds {get; set; }
        public bool AutoReschedule { get; set; } = false;
    }
}