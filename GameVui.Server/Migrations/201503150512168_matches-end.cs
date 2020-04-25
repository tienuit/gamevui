namespace GameVui.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class matchesend : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Matches", "GameEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Matches", "GameEnd", c => c.DateTime(nullable: false));
        }
    }
}
