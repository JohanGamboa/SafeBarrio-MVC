namespace SafeBarrio.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrearNotificaciones : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notificacions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Titulo = c.String(nullable: false),
                        Mensaje = c.String(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        Leida = c.Boolean(nullable: false),
                        UsuarioId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Notificacions");
        }
    }
}
