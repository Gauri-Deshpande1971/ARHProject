using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Services;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Authorize]
    public class OrganizationController : BaseWithUserApiController
    {
        IFormGridService<OrganizationDto> _fgs;
        ILogger<OrganizationController> _logger;

        public OrganizationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<OrganizationController> logger,
                IFormGridService<OrganizationDto> fgs
                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getorganizationslist")]
        public async Task<ActionResult<IReadOnlyList<OrganizationDto>>> GetOrganizationsList()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetOrganizationsAsync(currentuser);

            if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            {
                var ldxx = _mapper.Map<IReadOnlyList<Organization>, IReadOnlyList<OrganizationDto>>(ars);
                return Ok(ldxx);
            }

            //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();

            var ldx = _mapper.Map<IReadOnlyList<Organization>, IReadOnlyList<OrganizationDto>>(ars);
            
            return Ok(ldx);
        }
        

        [HttpPost("saveorganization")]
        public async Task<ActionResult<OrganizationDto>> SaveOrganization(OrganizationDto organization)
        {
            if (organization == null) {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!organization.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu =await GetCurrentUser();

	        Organization o = null;
            try
            {
                o = _mapper.Map<OrganizationDto, Organization>(organization);

                o = await _ms.ValidateOrganizationAsync(o, cu);
                if (o.Errors != null && !String.IsNullOrEmpty(o.Errors.errormessage))
                {
                    return BadRequest(new ApiResponse(401, o.Errors.errormessage));
                }

                o = await _ms.SaveOrganizationAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }

            return Ok(_mapper.Map<Organization, OrganizationDto>(o));
        }

        [HttpGet("templatedownload")]
        public async Task<ActionResult> OrganizationsDownloadTemplate()
        {
            var currentuser = await GetCurrentUser();

            Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
            DefaultValues.Add("IsActive", "Yes");

            var ret = await DownloadTemplate<OrganizationDto>("Organization", _fgs, DefaultValues);

            return ret;
        }

        [HttpGet("downloaddata")]
        public async Task<ActionResult> OrganizationsDownloadData()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetOrganizationsAsync(currentuser);

            if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            {
            }
            else
            {
                //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();
            }
            var ldx = _mapper.Map<IReadOnlyList<Organization>, IReadOnlyList<OrganizationDto>>(ars);

            var ret = await DownloadData<OrganizationDto>("Organization", _fgs, ldx);

            return ret;
        }

        [HttpPost("bulkupload")]
        public async Task<ActionResult> OrganizationsBulkUpload(CancellationToken cancellationToken)
        {
            var currentuser = await GetCurrentUser();

            string datefields = "CurrentStartDate,CurrentEndDate,ApplicationEndDate,ApprovalEndDate";

            var fgs = new FormGridService<OrganizationDto>(_fgs.GetUnitOfWork());

            var ret = await BulkUpload<OrganizationDto>(cancellationToken, Request.Form.Files[0], "Organization", fgs, datefields);
            //  var ret = await BulkUpload<OrganizationDto>(cancellationToken, Request.Form.Files[0], "Organization", _fgs);
            foreach(var v in ret.Headings) 
            {
                v.FieldName = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(v.FieldName);
            }

            if (ret == null)
            {
                return BadRequest(new ApiResponse(401, "Error processing file"));
            }

            if (!String.IsNullOrEmpty(ret.ErrorMessage))
            {
                return BadRequest(new ApiResponse(401, ret.ErrorMessage));
            }

            if (ret.DataSource == null || ret.DataSource.Count == 0)
            {
                return Ok(ret);
            }

            var valsrc = new ImportExcelData<Organization>();
            valsrc.ErrorInFields = ret.ErrorInFields;
            valsrc.Headings = ret.Headings;
            valsrc.DataSource = _mapper.Map<List<OrganizationDto>, List<Organization>>(ret.DataSource);

            valsrc = (ImportExcelData<Organization>) (await _ms.BulkValidateOrganizationAsync(valsrc, currentuser));
            if (!String.IsNullOrEmpty(valsrc.ErrorMessage))
            {
                return BadRequest(new ApiResponse(401, ret.ErrorMessage));
            }
            ret.ErrorInFields = valsrc.ErrorInFields;
            ret.Headings = valsrc.Headings;
            ret.DataSource = _mapper.Map<List<Organization>, List<OrganizationDto>>(valsrc.DataSource);

            SessionManager.Add("OrganizationUpload-" + currentuser.Id.ToString(), System.Text.Json.JsonSerializer.Serialize(ret));

            return Ok(ret);
        }

        [HttpPost("savebulkupload")]
        public async Task<ActionResult> SaveBulkUpload(string UploadCode)
        {
            var currentuser = await GetCurrentUser();

            var up = System.Text.Json.JsonSerializer.Deserialize<ImportExcelData<OrganizationDto>>(SessionManager.Get("OrganizationUpload-" + currentuser.Id.ToString()));

            if (up == null || up.ErrorInFields.Count > 0) {
                return BadRequest(new ApiResponse(401, "Cannot Save Data due to Errors !!"));
            }

            var oul = _mapper.Map<IReadOnlyList<OrganizationDto>, IReadOnlyList<Organization>>(up.DataSource);

            var ret = await _ms.SaveUploadOrganizationAsync(oul, currentuser);

            SessionManager.Delete("OrganizationUpload-" + currentuser.Id.ToString());

            return Ok();
        }

        [HttpGet("getgridcols")]
        public async Task<ActionResult> GetGridCols(string FormName)
        {
            var currentuser = await GetCurrentUser();

            if (FormName == "Organization" || FormName == "Organizations")
            {
                var gxl = await this.GetFormGridCols<OrganizationDto>(FormName, _fgs);
                return gxl;
            }

            return null;
        }

    }
}
