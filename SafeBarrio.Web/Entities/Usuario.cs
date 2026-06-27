using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SafeBarrio.Web.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        public bool EsAdmin { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Telefono { get; set; }

        public string Ubicacion { get; set; }

        public DateTime FechaRegistro { get; set; }
       
    }
}