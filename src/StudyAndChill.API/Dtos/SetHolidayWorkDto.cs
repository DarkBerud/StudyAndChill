using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class SetHolidayWorkDto
    {
        public DateOnly Date { get; set; }
        public bool WantsToWork { get; set; }
    }
}