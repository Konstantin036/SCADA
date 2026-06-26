using DataConcentrator.Models;
using System.Data.Entity;

namespace DataConcentrator.Database
{
    public class ScadaContext : DbContext
    {
        public ScadaContext() : base("name=ScadaDb") { }

        public DbSet<AnalogInput> AnalogInputs { get; set; }
        public DbSet<AnalogOutput> AnalogOutputs { get; set; }
        public DbSet<DigitalInput> DigitalInputs { get; set; }
        public DbSet<DigitalOutput> DigitalOutputs { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<ActivatedAlarm> ActivatedAlarms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnalogInput>().ToTable("AnalogInputs");
            modelBuilder.Entity<AnalogOutput>().ToTable("AnalogOutputs");
            modelBuilder.Entity<DigitalInput>().ToTable("DigitalInputs");
            modelBuilder.Entity<DigitalOutput>().ToTable("DigitalOutputs");
        }
    }
}