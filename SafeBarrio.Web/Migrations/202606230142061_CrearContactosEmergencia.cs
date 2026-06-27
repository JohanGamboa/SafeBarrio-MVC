namespace SafeBarrio.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrearContactosEmergencia : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactoEmergencias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        Telefono = c.String(nullable: false, maxLength: 20),
                        Parentesco = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuarios", t => t.UsuarioId, cascadeDelete: true)
                .Index(t => t.UsuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContactoEmergencias", "UsuarioId", "dbo.Usuarios");
            DropIndex("dbo.ContactoEmergencias", new[] { "UsuarioId" });
            DropTable("dbo.ContactoEmergencias");
        }
    }
}
