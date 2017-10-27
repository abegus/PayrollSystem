namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addHistory2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.History",
                c => new
                    {
                        Hid = c.String(nullable: false, maxLength: 128),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Pay = c.Decimal(storeType: "money"),
                        Level = c.Int(nullable: false),
                        Position = c.String(maxLength: 50),
                        Witholdings = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Mid = c.String(maxLength: 128),
                        DepartmentName = c.String(maxLength: 50),
                        ManagerName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Hid)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.History", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.History", new[] { "UserId" });
            DropTable("dbo.History");
        }
    }
}
