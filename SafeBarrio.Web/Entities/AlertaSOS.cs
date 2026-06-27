using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeBarrio.Web.Entities
{
    public class AlertaSOS
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public DateTime FechaAlerta { get; set; }

        public string Estado { get; set; }

        public string Mensaje { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }
}