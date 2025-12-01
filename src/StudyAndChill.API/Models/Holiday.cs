using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Models
{
    public class Holiday
    {
        public int Id {get; set; }

        [Required]
        public string Name {get; set; }

        [Required]
        public DateOnly Date {get; set; }

        public bool IsGlobal {get; set; } = true;

        public List<User> AffectedUsers {get; set; } = new();
    }
}