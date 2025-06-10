using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class FormGridController: BaseWithUserApiController
    {
        IGenericRepository<FormGridHeader> _fgh { get; set; }
        IGenericRepository<FormGridDetail> _fgd { get; set; }
        IFormGridService<FormGridDetail> _fgs {get; set;}

        public FormGridController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper, IMastersService ms,     
                //ILogger logger,
                IGenericRepository<FormGridHeader> fgh,
                IGenericRepository<FormGridDetail> fgd,
                IFormGridService<FormGridDetail> fgs
                ) : base(userManager, signInManager, mapper, ms)//, logger)
        {
            _fgd = fgd;
            _fgh = fgh;
            _fgs = fgs;
        }

        [HttpGet("formheaders")]
        public async Task<ActionResult> GetFormHeaders()
        {
            var fh = await _fgh.ListAllAsync();
            if (fh != null)
                fh = fh.ToList()
                    .OrderBy(x => x.FormName).ToList();
            return Ok(fh);
        }

        [HttpGet("formdetails")]
        public async Task<ActionResult> GetFormDetails(string FormName, string UserName)
        {
            var fd = await _fgd.ListAsync(new BaseSpecification<FormGridDetail>(x => x.FormGridHeader.FormName == FormName));
            if (fd != null)
                fd = fd.OrderBy(x => x.Position).ToList();

            return Ok(fd);
        }

        [HttpGet("refreshformdetails")]
        public async Task<ActionResult> RefreshFormDetails(string FormName, string UserName)
        {
            var fdx = await _fgs.RefreshFormGridDetails(FormName, -1);

            return Ok(fdx);
        }

        [HttpPost("saveformdetails")]
        public async Task<ActionResult> SaveFormDetails(FormGridUserDetailsDto fgdx)
        {
            var fd = await _fgs.UpdateFormGridDetail(fgdx.FormName, _mapper.Map<FormGridDetailDto, FormGridDetail>(fgdx.FormGridDetailData), fgdx.Username);

            return Ok();
        }

    }
}