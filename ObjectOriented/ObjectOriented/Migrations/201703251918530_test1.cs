namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Paystub", "Pay", c => c.Decimal(storeType: "money"));
            AlterColumn("dbo.Paystub", "NetPay", c => c.Decimal(storeType: "money"));
            AlterColumn("dbo.Paystub", "Tax", c => c.Decimal(storeType: "money"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Paystub", "Tax", c => c.Decimal(nullable: false, storeType: "money"));
            AlterColumn("dbo.Paystub", "NetPay", c => c.Decimal(nullable: false, storeType: "money"));
            AlterColumn("dbo.Paystub", "Pay", c => c.Decimal(nullable: false, storeType: "money"));
        }
    }
}
