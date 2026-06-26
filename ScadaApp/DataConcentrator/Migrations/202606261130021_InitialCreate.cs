namespace DataConcentrator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivatedAlarms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlarmId = c.Int(nullable: false),
                        TagName = c.String(),
                        Message = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Alarms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagName = c.String(),
                        Limit = c.Double(nullable: false),
                        Type = c.Int(nullable: false),
                        Message = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AnalogInputs",
                c => new
                    {
                        TagName = c.String(nullable: false, maxLength: 128),
                        LowLimit = c.Double(nullable: false),
                        HighLimit = c.Double(nullable: false),
                        Units = c.String(),
                        Deadband = c.Double(nullable: false),
                        Hysteresis = c.Double(nullable: false),
                        CurrentValue = c.Double(nullable: false),
                        ScanTime = c.Int(nullable: false),
                        IsScanning = c.Boolean(nullable: false),
                        Description = c.String(),
                        IOAddress = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TagName);
            
            CreateTable(
                "dbo.AnalogOutputs",
                c => new
                    {
                        TagName = c.String(nullable: false, maxLength: 128),
                        LowLimit = c.Double(nullable: false),
                        HighLimit = c.Double(nullable: false),
                        Units = c.String(),
                        CurrentValue = c.Double(nullable: false),
                        InitialValue = c.Double(nullable: false),
                        Description = c.String(),
                        IOAddress = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TagName);
            
            CreateTable(
                "dbo.DigitalInputs",
                c => new
                    {
                        TagName = c.String(nullable: false, maxLength: 128),
                        CurrentValue = c.Boolean(nullable: false),
                        ScanTime = c.Int(nullable: false),
                        IsScanning = c.Boolean(nullable: false),
                        Description = c.String(),
                        IOAddress = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TagName);
            
            CreateTable(
                "dbo.DigitalOutputs",
                c => new
                    {
                        TagName = c.String(nullable: false, maxLength: 128),
                        CurrentValue = c.Boolean(nullable: false),
                        InitialValue = c.Double(nullable: false),
                        Description = c.String(),
                        IOAddress = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TagName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DigitalOutputs");
            DropTable("dbo.DigitalInputs");
            DropTable("dbo.AnalogOutputs");
            DropTable("dbo.AnalogInputs");
            DropTable("dbo.Alarms");
            DropTable("dbo.ActivatedAlarms");
        }
    }
}
