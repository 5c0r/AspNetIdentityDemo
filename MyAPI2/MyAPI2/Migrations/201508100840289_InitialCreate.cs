namespace MyAPI2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Level");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Level", c => c.Byte(nullable: false));
        }
    }
}
