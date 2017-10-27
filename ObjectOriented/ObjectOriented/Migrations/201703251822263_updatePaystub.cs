namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePaystub : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Paystub", "NetPay", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Paystub", "Tax", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Paystub", "Mid", c => c.String(maxLength: 128));
            AddColumn("dbo.Paystub", "DepartmentName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Paystub", "DepartmentName");
            DropColumn("dbo.Paystub", "Mid");
            DropColumn("dbo.Paystub", "Tax");
            DropColumn("dbo.Paystub", "NetPay");
        }
    }
}
