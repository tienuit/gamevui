namespace GameVui.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class point : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Point", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Point");
        }
    }
}
