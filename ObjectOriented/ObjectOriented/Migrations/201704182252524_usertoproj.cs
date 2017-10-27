namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usertoproj : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DepToProjs", "ProjId", "dbo.Project");
            CreateTable(
                "dbo.UserToProjs",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        ProjId = c.String(nullable: false, maxLength: 128),
                        Temp = c.String(unicode: false),
                        Percentage = c.Decimal(nullable: false, precision: 19, scale: 4),
                    })
                .PrimaryKey(t => new { t.UserId, t.ProjId })
                .ForeignKey("dbo.Project", t => t.ProjId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ProjId);
            
            AddForeignKey("dbo.DepToProjs", "ProjId", "dbo.Project", "ProjId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DepToProjs", "ProjId", "dbo.Project");
            DropForeignKey("dbo.UserToProjs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserToProjs", "ProjId", "dbo.Project");
            DropIndex("dbo.UserToProjs", new[] { "ProjId" });
            DropIndex("dbo.UserToProjs", new[] { "UserId" });
            DropTable("dbo.UserToProjs");
            AddForeignKey("dbo.DepToProjs", "ProjId", "dbo.Project", "ProjId", cascadeDelete: true);
        }
    }
}
