using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudyAndChill.API.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Oopsie! Tem algo faltando.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email {get; set;} = string.Empty;

        [Required(ErrorMessage = "Oopsie! Tem algo faltando.")]
        public string Password {get; set;} = string.Empty;
    }
}