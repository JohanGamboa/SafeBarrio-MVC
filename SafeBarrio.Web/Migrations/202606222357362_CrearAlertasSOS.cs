namespace SafeBarrio.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrearAlertasSOS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AlertaSOS",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        Latitud = c.Double(nullable: false),
                        Longitud = c.Double(nullable: false),
                        FechaAlerta = c.DateTime(nullable: false),
                        Estado = c.String(),
                        Mensaje = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuarios", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.UsuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlertaSOS", "UsuarioId", "dbo.Usuarios");
            DropIndex("dbo.AlertaSOS", new[] { "UsuarioId" });
            DropTable("dbo.AlertaSOS");
        }
    }
}
