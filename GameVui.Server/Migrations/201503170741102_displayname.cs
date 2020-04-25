namespace GameVui.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class displayname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DisplayName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DisplayName");
        }
    }
}
