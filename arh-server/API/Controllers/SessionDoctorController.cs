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
    public class SessionDoctorController:BaseWithUserApiController
    {
        IFormGridService<SessionDoctorsDto> _fgs;
        ILogger<SessionDoctorController> _logger;
        public SessionDoctorController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<SessionDoctorController> logger,
                IFormGridService<SessionDoctorsDto> fgs
                                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getSessionDoctorslist")]
        public async Task<ActionResult<IReadOnlyList<SessionSetupDto>>> GetSessionDoctorsList(int id)
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetSessionDoctorsAsync(id);

            var ldx = _mapper.Map<IReadOnlyList<SessionDoctors>, IReadOnlyList<SessionDoctorsDto>>(ars);

            return Ok(ldx);
        }
        [HttpPost("savesessionDoctors")]
        public async Task<ActionResult<IEnumerable<SessionDoctorsDto>>> SaveSessionDoctors([FromBody] IEnumerable<SessionDoctorsDto> sessionList)
        {
            if (sessionList == null || !sessionList.Any())
            {
                return BadRequest(new ApiResponse(401, "No user info received !!"));
            }

            var currentUser = await GetCurrentUser();
            try
            {
                // Map DTOs to Entities
                var doctors = _mapper.Map<IEnumerable<SessionDoctorsDto>, IEnumerable<SessionDoctors>>(sessionList);

                // Validate each SessionDoctor
                var validatedDoctors = await _ms.ValidateSessionDoctorsAsync(doctors, currentUser);

                // Collect any validation errors
                var errors = validatedDoctors
                    .Where(d => d.Errors != null && !string.IsNullOrWhiteSpace(d.Errors.errormessage))
                    .Select(d => d.Errors.errormessage)
                    .ToList();

                if (errors.Any())
                {
                    return BadRequest(new ApiResponse(401, string.Join(" | ", errors)));
                }

                // Save all validated session doctors
                var savedDoctors = await _ms.SaveSessionDoctorsAsync(validatedDoctors);

                if (savedDoctors == null || !savedDoctors.Any())
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }

                // Map to DTOs and return
                var resultDtos = _mapper.Map<IEnumerable<SessionDoctors>, IEnumerable<SessionDoctorsDto>>(savedDoctors);
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

            if (FormName == "SessionDoctor" || FormName == "sessiondoctor")
            {
                var fgs = new FormGridService<SessionDoctorsDto>(_fgs.GetUnitOfWork());
                var columnResult = await this.GetFormGridCols<SessionDoctorsDto>(FormName, fgs);
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
                var sessionDoctors = await _fgs.GetUnitOfWork().Repository<SessionDoctors>()
                    .GetEntityListWithSpec(new BaseSpecification<SessionDoctors>(
                        x => x.SessionId == todaySession.Id
                    ));                              

                var rows = _mapper.Map<IReadOnlyList<SessionDoctors>, IReadOnlyList<SessionDoctorsDto>>(sessionDoctors);
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
