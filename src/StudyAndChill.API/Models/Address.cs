using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudyAndChill.API.Models
{
    [Owned]
    public class Address
    {
        [Required]
        public string Country { get; set; } = "Brasil";

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string Number { get; set; } = string.Empty;

        public string? Complement { get; set; }

        [Required]
        public string Neighborhood { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;
    }
}