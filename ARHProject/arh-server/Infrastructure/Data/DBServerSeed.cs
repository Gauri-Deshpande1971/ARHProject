using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Identity;
using Core.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class DBServerSeed
    {
        public static async Task SeedAsync(DBServerContext context, ILoggerFactory loggerFactory, 
                AppIdentityDbContext appcontext, UserManager<AppUser> userManager)
        {
            try
            {
                if (!context.SysDatas.Any())
                {
                    context.SysDatas.Add(new SysData() {
                        FieldName = "COMPANY_NAME",
                        FieldValue = "DEMO COMPANY"
                    });
                    context.SysDatas.Add(new SysData() {
                        FieldName = "TODAY",
                        FieldValue = string.Format("{0:yyyy-MM-dd}", DateTime.Today)
                    });
                    context.SysDatas.Add(new SysData() {
                        FieldName = "DEFAULT_EMAIL_DOMAIN",
                        FieldValue = "leegansoftwares.com"
                    });

                    await context.SaveChangesAsync();
                }
                
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            
                if (!context.AppRoles.Any())
                {
                    if (File.Exists(path + @"/Data/SeedData/approles.json")) 
                    {
                        var vt = File.ReadAllText(path + @"/Data/SeedData/approles.json");
                        if (vt != null) {
                            var appRoles = JsonSerializer.Deserialize<List<AppRole>>(vt);
                            foreach(var vtx in appRoles)
                            {
                                context.AppRoles.Add(vtx);
                            }

                            await context.SaveChangesAsync();
                        }
                    }
                }

                if (!context.NavMenus.Any())
                {
                    if (File.Exists(path + @"/Data/SeedData/navmenu.json")) 
                    {
                        var nm = File.ReadAllText(path + @"/Data/SeedData/navmenu.json");
                        if (nm != null) {
                            var navMenus = JsonSerializer.Deserialize<List<NavMenu>>(nm);
                            foreach(var vtx in navMenus)
                            {
                                context.NavMenus.Add(vtx);
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }

                if (!context.Organizations.Any())
                {
                    context.Organizations.Add(new Organization() {
                        CreatedById= 1,
                        CreatedByName = "Administrator",
                        CreatedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false,
                        OrganizationName = "Association of Research in Homeopathy",
                        UCode = Guid.NewGuid()
                    });

                    await context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                var log = loggerFactory.CreateLogger<DBServerSeed>();
                log.LogError(ex.Message);
            }
        }
    }
}
