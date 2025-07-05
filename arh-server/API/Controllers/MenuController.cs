using API.Dtos;
using AutoMapper;
using Core.Entities.Identity;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class MenuController : BaseWithUserApiController
    {
        ILogger<PatientController> _logger;

        public MenuController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<PatientController> logger,
                IFormGridService<patientDto> fgs
                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("getnavmenus")]      
       // Optional: Use if JWT/Auth is implemented
        public async Task<ActionResult<IReadOnlyList<NavmenuOfUser>>> GetUserNavMenus()
        {
            var currentuser = await GetCurrentUser();
            if (string.IsNullOrEmpty(currentuser.Id))
            {
                return BadRequest("AppUserId is required.");
            }

            var result = await _ms.GetNavmenuOfUserAsync(currentuser.Id);

            if (result == null || result.Count == 0)
            {
                return NotFound("No navigation menus found for the user.");
            }

            return Ok(result);
        }
    }

}

