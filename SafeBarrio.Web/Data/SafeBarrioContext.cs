using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SafeBarrio.Web.Entities;

namespace SafeBarrio.Web.Data
{
    public class SafeBarrioContext : DbContext
    {
        public SafeBarrioContext() : base("SafeBarrioConnection")
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Incidente> Incidentes { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<AlertaSOS> AlertasSOS { get; set; }
        public DbSet<ContactoEmergencia> ContactosEmergencia { get; set; }
    }
}