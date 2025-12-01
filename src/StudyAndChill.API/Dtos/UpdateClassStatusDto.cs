using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StudyAndChill.API.Enums;

namespace StudyAndChill.API.Dtos
{
    public class UpdateClassStatusDto
    {
        [Required]
        [EnumDataType(typeof(ClassStatus))]
        public ClassStatus NewStatus { get; set; }
    }
}