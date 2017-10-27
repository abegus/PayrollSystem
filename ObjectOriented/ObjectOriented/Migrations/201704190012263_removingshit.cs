namespace ObjectOriented.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingshit : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Department", "check");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Department", "check", c => c.Boolean(nullable: false));
        }
    }
}
