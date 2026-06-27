using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeBarrio.Web.Entities
{
    public class Incidente
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoIncidente { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public double Latitud { get; set; }

        [Required]
        public double Longitud { get; set; }

        public string DireccionReferencia { get; set; }

        public string ImagenRuta { get; set; }

        public DateTime FechaReporte { get; set; }

        public string Estado { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
        
    }
}