﻿using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Specifications;
using Infrastructure.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace API.Controllers
{
    public class SessionSetupController : BaseWithUserApiController
    {
        IFormGridService<SessionSetupDto> _fgs;
        ILogger<SessionSetupController> _logger;
        //  IGoogleCalendarService _googleCalendarService;
        public SessionSetupController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<SessionSetupController> logger,
                IFormGridService<SessionSetupDto> fgs
                                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getSessionslist")]
        public async Task<ActionResult<IReadOnlyList<SessionSetupDto>>> GetSessionsList()
        {
            var currentuser = await GetCurrentUser();
            var ars = await _ms.GetSessionsAsync();

            var ldx = _mapper.Map<IReadOnlyList<SessionSetup>, IReadOnlyList<SessionSetupDto>>(ars);

            return Ok(ldx);
        }

        [HttpPost("savesession")]
        public async Task<ActionResult<SessionSetupDto>> SaveSession(SessionSetupDto session)
        {
            if (session == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!session.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }
            var cu = await GetCurrentUser();

            SessionSetup a = null;
            try
            {
                a = _mapper.Map<SessionSetupDto, SessionSetup>(session);

                a = await _ms.ValidateSessionAsync(a, cu);
                if (a.Errors != null && !String.IsNullOrEmpty(a.Errors.errormessage))
                {
                    return BadRequest(new ApiResponse(401, a.Errors.errormessage));
                }

                a = await _ms.SaveSessionAsync(a);
                if (a == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }

            return Ok(_mapper.Map<SessionSetup, SessionSetupDto>(a));
        }

        [HttpGet("getgridcols")]
        public async Task<ActionResult> GetGridCols(string FormName)
        {
            var currentuser = await GetCurrentUser();

            if (FormName == "Session" || FormName == "session")
            {
                var fgs = new FormGridService<sessionDetailsDto>(_fgs.GetUnitOfWork());
                var columnResult = await this.GetFormGridCols<sessionDetailsDto>(FormName, fgs);
                if (columnResult is not OkObjectResult okColumnResult)
                    return BadRequest("Could not get column metadata.");

                var columns = okColumnResult.Value as IReadOnlyList<FormGridDetailDto>;
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);

                var sessionEntities = await _fgs.GetUnitOfWork().Repository<SessionSetup>()
                                      .GetEntityListWithSpec(new BaseSpecification<SessionSetup>(
                    x => x.SessionDate >= today && x.SessionDate < tomorrow)
                );

                var appUsers = await _userManager.Users.ToListAsync();
                var sessionIds = sessionEntities.Select(s => s.Id).ToList();

                var docsInSession = await _fgs.GetUnitOfWork().Repository<SessionDoctors>()
                    .GetEntityListWithSpec(new BaseSpecification<SessionDoctors>(x => sessionIds.Contains(x.SessionId)));

                var dispenseTeamInSession = await _fgs.GetUnitOfWork().Repository<SessionDispenseTeam>()
                    .GetEntityListWithSpec(new BaseSpecification<SessionDispenseTeam>(x => sessionIds.Contains(x.SessionId)));

                var sessionDetailsList = sessionEntities.Select(sess => new sessionDetailsDto
                {
                    SessionId = sess.Id,
                    SessionName = sess.SessionName,
                    SessionDate = sess.SessionDate,
                    IsActive=sess.IsActive,
                    SessionDoctors = docsInSession
                        .Where(d => d.SessionId == sess.Id)
                        .Select(d =>
                        {
                            var user = appUsers.FirstOrDefault(u => u.OfficeUserId == d.DoctorId);
                            return new SessionDoctorsDto
                            {
                                DoctorId = d.DoctorId,
                                DisplayName = user?.DisplayName,
                                SessionId=sess.Id,
                                UCode= d.UCode,
                            };
                        }).ToList(),

                    SessionDispenses = dispenseTeamInSession
                        .Where(d => d.SessionId == sess.Id)
                        .Select(d =>
                        {
                            var user = appUsers.FirstOrDefault(u => u.OfficeUserId == d.MemberId);
                            return new SessionDispenseTeamDto
                            {
                                MemberId = d.MemberId,
                                DisplayName = user?.DisplayName,
                                SessionId = sess.Id,
                                UCode = d.UCode,
                            };
                        }).ToList()
                }).ToList();
                return Ok(new
                {
                    Columns = columns,
                    Rows = sessionDetailsList
                });
            }
            return BadRequest();
        }

    }
}


