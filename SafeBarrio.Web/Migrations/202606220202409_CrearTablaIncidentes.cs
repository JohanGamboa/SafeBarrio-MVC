namespace SafeBarrio.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrearTablaIncidentes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Incidentes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        TipoIncidente = c.String(nullable: false, maxLength: 50),
                        Descripcion = c.String(nullable: false, maxLength: 500),
                        Latitud = c.Double(nullable: false),
                        Longitud = c.Double(nullable: false),
                        DireccionReferencia = c.String(),
                        ImagenRuta = c.String(),
                        FechaReporte = c.DateTime(nullable: false),
                        Estado = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuarios", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.UsuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Incidentes", "UsuarioId", "dbo.Usuarios");
            DropIndex("dbo.Incidentes", new[] { "UsuarioId" });
            DropTable("dbo.Incidentes");
        }
    }
}
