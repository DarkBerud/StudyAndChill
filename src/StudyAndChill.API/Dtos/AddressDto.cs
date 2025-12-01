using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class AddressDto
    {
        [Required]
        public string Country { get; set; } = "Brasil";

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Number { get; set; }

        public string? Complement { get; set; }

        [Required]
        public string Neighborhood { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

    }
}