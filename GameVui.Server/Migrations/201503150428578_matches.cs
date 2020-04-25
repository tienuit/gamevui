namespace GameVui.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class matches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        PlayerId1 = c.String(nullable: false, maxLength: 128),
                        PlayerId2 = c.String(nullable: false, maxLength: 128),
                        GameBegin = c.DateTime(nullable: false),
                        GameEnd = c.DateTime(nullable: false),
                        Player1Achievement = c.Int(nullable: false),
                        Player2Achievement = c.Int(nullable: false),
                        WinnerId = c.String(maxLength: 128),
                        Note = c.String(),
                    })
                .PrimaryKey(t => new { t.PlayerId1, t.PlayerId2, t.GameBegin })
                .ForeignKey("dbo.AspNetUsers", t => t.PlayerId1)
                .ForeignKey("dbo.AspNetUsers", t => t.PlayerId2)
                .ForeignKey("dbo.AspNetUsers", t => t.WinnerId)
                .Index(t => t.PlayerId1)
                .Index(t => t.PlayerId2)
                .Index(t => t.WinnerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Matches", "WinnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Matches", "PlayerId2", "dbo.AspNetUsers");
            DropForeignKey("dbo.Matches", "PlayerId1", "dbo.AspNetUsers");
            DropIndex("dbo.Matches", new[] { "WinnerId" });
            DropIndex("dbo.Matches", new[] { "PlayerId2" });
            DropIndex("dbo.Matches", new[] { "PlayerId1" });
            DropTable("dbo.Matches");
        }
    }
}
