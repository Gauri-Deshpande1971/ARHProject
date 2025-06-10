using System.Threading.Tasks;
using API.Extensions;
using API.Dtos;
using Core.Entities.Identity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Core.Interfaces;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {


        // public async Task<AppUser> FindCurrentUser()
        // {
        //     // //var email = User.FindFirstValue(ClaimTypes.Email);
            
        //     // var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?
        //     //     .Value;
        //     // var user = await _userManager.FindByEmailAsync(email);

        //     var user = await _userManager.FindEmailFromClaimsPrinciple(HttpContext.User);

        //     return user;
        // }

    }
}
