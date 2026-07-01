namespace DataConcentrator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagValueHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagValueHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(),
                        Value = c.Double(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TagValueHistories");
        }
    }
}
