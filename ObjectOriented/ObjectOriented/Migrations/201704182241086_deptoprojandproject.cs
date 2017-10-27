namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deptoprojandproject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DepToProjs",
                c => new
                    {
                        DepId = c.String(nullable: false, maxLength: 128),
                        ProjId = c.String(nullable: false, maxLength: 128),
                        Temp = c.String(unicode: false),
                    })
                .PrimaryKey(t => new { t.DepId, t.ProjId })
                .ForeignKey("dbo.Project", t => t.ProjId, cascadeDelete: true)
                .ForeignKey("dbo.Department", t => t.DepId)
                .Index(t => t.DepId)
                .Index(t => t.ProjId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ProjId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                        OtherCosts = c.Decimal(nullable: false, storeType: "money"),
                        Total = c.Decimal(nullable: false, storeType: "money"),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProjId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DepToProjs", "DepId", "dbo.Department");
            DropForeignKey("dbo.DepToProjs", "ProjId", "dbo.Project");
            DropIndex("dbo.DepToProjs", new[] { "ProjId" });
            DropIndex("dbo.DepToProjs", new[] { "DepId" });
            DropTable("dbo.Project");
            DropTable("dbo.DepToProjs");
        }
    }
}
