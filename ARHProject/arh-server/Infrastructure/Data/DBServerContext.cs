using System.Reflection;
using Core.Entities;
using Core.Entities.Identity;
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

        public DbSet<NavMenu> NavMenus { get; set; }
        public DbSet<MailConfig> MailConfigs { get; set; }
        public DbSet<MailLog> MailLogs { get; set; }

        public DbSet<FormGridHeader> FormGridHeaders { get; set; }
        public DbSet<FormGridDetail> FormGridDetails { get; set; }
        public DbSet<UserNavMenu> UserNavMenus { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        
            // Apply configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
