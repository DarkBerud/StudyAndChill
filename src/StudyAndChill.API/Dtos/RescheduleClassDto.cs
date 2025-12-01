using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class RescheduleClassDto
    {
        [Required(ErrorMessage = "A nova data e hora de aula são obrigatórias.")]
        public DateTime NewStartTime {get; set;}
    }
}