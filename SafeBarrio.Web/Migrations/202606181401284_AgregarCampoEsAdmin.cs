namespace SafeBarrio.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarCampoEsAdmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuarios", "EsAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuarios", "EsAdmin");
        }
    }
}
