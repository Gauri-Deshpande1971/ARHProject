using API.Dtos;
using AutoMapper;
using Core.Entities.Identity;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CityController:BaseWithUserApiController
    {
        IFormGridService<cityDto> _fgs;
        ILogger<CityController> _logger;
        IHttpContextAccessor _contextAccessor;
        public CityController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<CityController> logger,
                IFormGridService<cityDto> fgs
                                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getcitieslist")]
        public async Task<ActionResult<IReadOnlyList<cityDto>>> GetCitiesList()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetCitiesAsync(currentuser);

            var ldx = _mapper.Map<IReadOnlyList<City>, IReadOnlyList<cityDto>>(ars);

            return Ok(ldx);
        }

    }
}

