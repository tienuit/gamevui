namespace GameVui.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupmessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupMessages",
                c => new
                    {
                        SenderId = c.String(nullable: false, maxLength: 128),
                        CreatedTime = c.DateTime(nullable: false),
                        MessageContent = c.String(),
                    })
                .PrimaryKey(t => new { t.SenderId, t.CreatedTime })
                .ForeignKey("dbo.AspNetUsers", t => t.SenderId, cascadeDelete: true)
                .Index(t => t.SenderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupMessages", "SenderId", "dbo.AspNetUsers");
            DropIndex("dbo.GroupMessages", new[] { "SenderId" });
            DropTable("dbo.GroupMessages");
        }
    }
}
