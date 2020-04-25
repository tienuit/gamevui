namespace GameVui.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bestmatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "TotalNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "Finished", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "Finished");
            DropColumn("dbo.Matches", "TotalNumber");
        }
    }
}
