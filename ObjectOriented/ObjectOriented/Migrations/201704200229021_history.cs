namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class history : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.History", "StartDate", c => c.DateTime());
            AlterColumn("dbo.History", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.History", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.History", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
