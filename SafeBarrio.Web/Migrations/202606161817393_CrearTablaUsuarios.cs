namespace SafeBarrio.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrearTablaUsuarios : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        Apellido = c.String(nullable: false, maxLength: 100),
                        Correo = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Telefono = c.String(),
                        Ubicacion = c.String(),
                        FechaRegistro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Usuarios");
        }
    }
}
