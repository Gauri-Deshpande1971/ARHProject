using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindByEmailAsync(this UserManager<AppUser> input, ClaimsPrincipal user)
        {
            var username = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?
                .Value;
            
            //return await input.Users.Include(x => x.Address).Where(x => x.Email == email).FirstOrDefault();

            return await input.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
        }

        public static async Task<AppUser> FindEmailFromClaimsPrinciple(this UserManager<AppUser> input, ClaimsPrincipal user)
        {
            var username = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?
                .Value;

            return await input.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
        }

        public static async Task<AppUser> FindUserFromClaimsPrinciple(this UserManager<AppUser> input, string username)
        {
            var t = await input.Users.Where(x => x.UserName == username).ToListAsync();

            if (t == null || t.Count() == 0)
            {
                return null;
            }

            return t.FirstOrDefault();
        }

        public static async Task<AppUser> FindUserFromClaimsNameAndRole(this UserManager<AppUser> input, ClaimsPrincipal user)
        {
            var username = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var approlename = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

            return await input.Users.Where(x => x.UserName == username && x.AppRoleCode == approlename).FirstOrDefaultAsync();
        }    
        public static async Task<AppUser> FindUserFromClaimsNameAndRole(this UserManager<AppUser> input, string username, string approlename)
        {
            var t = await input.Users.Where(x => x.UserName == username).ToListAsync();

            if (t == null || t.Count() == 0)
            {
                return null;
            }

            return t.FirstOrDefault();

        }

        public static async Task<IReadOnlyList<AppUser>> UsersListAsync(this UserManager<AppUser> input)
        {
            return await input.Users.ToListAsync();
        }


    }
}
