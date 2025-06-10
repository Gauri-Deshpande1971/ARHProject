using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
        //     if (!userManager.Users.Any())
        //     {
        //         // var user = new AppUser
        //         // {
        //         //     DisplayName = "Administrator",
        //         //     Email = "admin@leegansoftwares.com",
        //         //     UserName = "admin",
        //         //     AppRoleCode = "ADMINISTRATOR"
        //         // };

        //         // await userManager.CreateAsync(user, "Pass@1234");
        //     }
        }
    }
}