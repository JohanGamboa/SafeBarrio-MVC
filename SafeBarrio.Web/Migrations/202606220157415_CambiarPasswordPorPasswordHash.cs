namespace SafeBarrio.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiarPasswordPorPasswordHash : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuarios", "PasswordHash", c => c.String(nullable: false));
            DropColumn("dbo.Usuarios", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Usuarios", "Password", c => c.String(nullable: false));
            DropColumn("dbo.Usuarios", "PasswordHash");
        }
    }
}
