using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CountryController:BaseWithUserApiController
    {
        IFormGridService<countryDto> _fgs;
        ILogger<CountryController> _logger;
        IHttpContextAccessor _contextAccessor;
        public CountryController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<CountryController> logger,
                IFormGridService<countryDto> fgs
                                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getcountrieslist")]
        public async Task<ActionResult<IReadOnlyList<countryDto>>> GetCountriesList()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetCountriesAsync();

            var ldx = _mapper.Map<IReadOnlyList<Country>, IReadOnlyList<countryDto>>(ars);

            return Ok(ldx);
        }

    }
}
