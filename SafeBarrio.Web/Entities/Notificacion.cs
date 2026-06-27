using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SafeBarrio.Web.Entities
{
    public class Notificacion
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public DateTime Fecha { get; set; }

        public bool Leida { get; set; }

        public int UsuarioId { get; set; }
    }
}