using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SafeBarrio.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Password { get; set; }
    }
}