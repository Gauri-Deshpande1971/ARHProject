using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities.Identity;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class SesssionDispenseTeamController : BaseWithUserApiController
    {
        IFormGridService<SessionDispenseTeamDto> _fgs;
        ILogger<SesssionDispenseTeamController> _logger;
        public SesssionDispenseTeamController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<SesssionDispenseTeamController> logger,
                IFormGridService<SessionDispenseTeamDto> fgs
                                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getSessionDispenseTeamlist")]
        public async Task<ActionResult<IReadOnlyList<SessionDispenseTeamDto>>> GetSessionDispenseTeamsList(int id)
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetSessionDispenseTeamAsync(currentuser, id);

            //if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            //{
            //    var ldxx = _mapper.Map<IReadOnlyList<SessionSetup>, IReadOnlyList<SessionSetupDto>>(ars);
            //    return Ok(ldxx);
            //}

            //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();

            var ldx = _mapper.Map<IReadOnlyList<SessionDispenseTeam>, IReadOnlyList<SessionDispenseTeam>>(ars);

            return Ok(ldx);
        }


        [HttpPost("savesessionDispenseTeam")]
        public async Task<ActionResult<IEnumerable<SessionDispenseTeamDto>>> SaveSessionDoctors( IEnumerable<SessionDispenseTeamDto> sessionList)
        {
            if (sessionList == null || !sessionList.Any())
            {
                return BadRequest(new ApiResponse(401, "No user info received !!"));
            }

            var currentUser = await GetCurrentUser();

            try
            {
                // Map DTOs to Entities
                var team = _mapper.Map<IEnumerable<SessionDispenseTeamDto>, IEnumerable<SessionDispenseTeam>>(sessionList);

                // Validate each SessionDoctor
                var validatedDispenseTeam = await _ms.ValidateSessionDispenseTeamAsync(team, currentUser);

                // Collect any validation errors
                var errors = validatedDispenseTeam
                    .Where(d => d.Errors != null && !string.IsNullOrWhiteSpace(d.Errors.errormessage))
                    .Select(d => d.Errors.errormessage)
                    .ToList();

                if (errors.Any())
                {
                    return BadRequest(new ApiResponse(401, string.Join(" | ", errors)));
                }

                // Save all validated session doctors
                var savedDispense = await _ms.SaveSessionDispenseTeamAsync(validatedDispenseTeam);

                if (savedDispense == null || !savedDispense.Any())
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }

                // Map to DTOs and return
                var resultDtos = _mapper.Map<IEnumerable<SessionDispenseTeam>, IEnumerable<SessionDispenseTeamDto>>(savedDispense);
                return Ok(resultDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
        }

        [HttpGet("getgridcols")]
        public async Task<ActionResult> GetGridCols(string FormName)
        {
            var currentuser = await GetCurrentUser();

            if (FormName == "SessionDispenseTeam" || FormName == "sessiondispenseteam")
            {
                var fgs = new FormGridService<SessionDispenseTeamDto>(_fgs.GetUnitOfWork());
                var columnResult = await this.GetFormGridCols<SessionDispenseTeamDto>(FormName, fgs);
                if (columnResult is not OkObjectResult okColumnResult)
                    return BadRequest("Could not get column metadata.");

                var columns = okColumnResult.Value as IReadOnlyList<FormGridDetailDto>;
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);
                var todaySession = await _fgs.GetUnitOfWork().Repository<SessionSetup>()
     .GetEntityWithSpec(new BaseSpecification<SessionSetup>(
         s => s.SessionDate >= today && s.SessionDate < tomorrow && s.IsActive == true
     ));

                if (todaySession == null) return NotFound("No active session found.");

                // Step 2: Get doctors for that session
                var sessionDispenseTeam = await _fgs.GetUnitOfWork().Repository<SessionDispenseTeam>()
                    .GetEntityListWithSpec(new BaseSpecification<SessionDispenseTeam>(
                        x => x.SessionId == todaySession.Id
                    ));
                var rows = _mapper.Map<IReadOnlyList<SessionDispenseTeam>, IReadOnlyList<SessionDispenseTeamDto>>(sessionDispenseTeam);
                return Ok(new
                {
                    Columns = columns,
                    Rows = rows
                });

            }
            return null;
        }
    }

}
