using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Dtos
{
    public class CreateContractDto
    {
        [Required] public int StudentId { get; set; }
        [Required] public int TeacherId { get; set; }
        [Required] public ContractType Type { get; set; }
        [Required] public ClassDuration ClassDuration { get; set; }
        [Required] public decimal MonthlyAmount { get; set; }
        [Required] public decimal TeacherPaymentShare { get; set; }
        [Required] public int DueDay { get; set; }
        [Required] public DateOnly StartDate { get; set; }
        [Required] public DateOnly EndDate { get; set; }
    }
}