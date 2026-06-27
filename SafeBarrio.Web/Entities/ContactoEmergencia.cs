using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeBarrio.Web.Entities
{
    public class ContactoEmergencia
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(50)]
        public string Parentesco { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }
}