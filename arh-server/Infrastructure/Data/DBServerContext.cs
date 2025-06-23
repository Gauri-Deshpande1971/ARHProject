using System.Reflection;
using Core.Entities;
using Core.Entities.Identity;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DBServerContext : DbContext
    {
        public DBServerContext(DbContextOptions<DBServerContext> options) : base(options)
        {
        }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Rate> Rate { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<SysData> SysDatas {get; set; }
        public DbSet<Core.Entities.Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Core.Entities.City> Cities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<NavMenu> NavMenus { get; set; }
        public DbSet<MailConfig> MailConfigs { get; set; }
        public DbSet<MailLog> MailLogs { get; set; }
        public DbSet<FormGridHeader> FormGridHeaders { get; set; }
        public DbSet<FormGridDetail> FormGridDetails { get; set; }
        public DbSet<OfficeUser> OfficeUser { get; set; }
        public DbSet<Core.Entities.Department> Department { get; set; }
        public DbSet<Core.Entities.Designation> designations { get; set; }
        public DbSet<UserNavMenu> UserNavMenus { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Core.Entities.Category> Categories { get; set; }
        public DbSet<patient> patient { get; set; }
        public DbSet<appointments> appointments { get; set; }
        public DbSet<appointmentMilestone> appointmentMilestone { get; set; }
        public DbSet<medicineKit> medicineKit { get; set; }
        public DbSet<Medicine> Medicine { get; set; }  
        public DbSet<potency> potency { get; set; }
        public DbSet<dosage> dosage { get; set; }
        public DbSet<clinical_diagnosis> clinical_diagnosis { get; set; }
        public DbSet<complaints> complaints { get; set; }
        public DbSet<physicalgen> physicalgen { get; set; }
        public DbSet<pasthistory> pasthistory { get; set; }
        public DbSet<family> family { get; set; }
        public DbSet<physicalexam> physicalexam { get; set; }
        public DbSet<systemeticexam> systemeticexam { get; set; }
        public DbSet<medications> medications { get; set; }
        public DbSet<investigations> investigations { get; set; }
        public DbSet<additionalreports> additionalreports { get; set; }
        public DbSet<remedy_plans> remedy_plans { get; set; }
        public DbSet<opd> opd { get; set; }
        public DbSet<opd_doc> opd_doc { get; set; }
        public DbSet<prescription> prescription { get; set; }
        public DbSet<SessionSetup> SessionSetup { get; set; }
        public DbSet<SessionDoctors> SessionDoctors { get; set; }
        public DbSet<SessionDispenseTeam> SessionDispenseTeam { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        
            // Apply configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
