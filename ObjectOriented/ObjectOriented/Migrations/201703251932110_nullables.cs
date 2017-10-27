namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Salary", c => c.Decimal(nullable: false, storeType: "money"));
            AlterColumn("dbo.AspNetUsers", "Witholdings", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Witholdings", c => c.Int());
            AlterColumn("dbo.AspNetUsers", "Salary", c => c.Decimal(storeType: "money"));
        }
    }
}
