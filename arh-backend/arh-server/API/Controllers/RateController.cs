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

namespace API.Controllers
{
    [Authorize]
    public class RateController : BaseWithUserApiController
    {
        IFormGridService<RateDto> _fgs;
        ILogger<RateController> _logger;

        public RateController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<RateController> logger,
                IFormGridService<RateDto> fgs
                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getrateslist")]
        public async Task<ActionResult<IReadOnlyList<RateDto>>> GetRatesList()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetRatesAsync(currentuser);

            //if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            //{
            //    var ldxx = _mapper.Map<IReadOnlyList<Rate>, IReadOnlyList<RateDto>>(ars);
            //    return Ok(ldxx);
            //}

            //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();

            var ldx = _mapper.Map<IReadOnlyList<Rate>, IReadOnlyList<RateDto>>(ars);

            return Ok(ldx);
        }


        [HttpPost("saverate")]
        public async Task<ActionResult<RateDto>> SaveRate(RateDto rate)
        {
            if (rate == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!rate.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();

            Rate r = null;
            try
            {
                r = _mapper.Map<RateDto, Rate>(rate);

                r = await _ms.ValidateRateAsync(r, cu,r.TypeOfCharges);
                if (r.Errors != null && !String.IsNullOrEmpty(r.Errors.errormessage))
                {
                    return BadRequest(new ApiResponse(401, r.Errors.errormessage));
                }

                r = await _ms.SaveRateAsync(r);
                if (r == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }

            return Ok(_mapper.Map<Rate, RateDto>(r));
        }

        [HttpGet("templatedownload")]
        public async Task<ActionResult> RatesDownloadTemplate()
        {
            var currentuser = await GetCurrentUser();

            Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
            DefaultValues.Add("IsActive", "Yes");

            var ret = await DownloadTemplate<RateDto>("Rate", _fgs, DefaultValues);

            return ret;
        }

        [HttpGet("downloaddata")]
        public async Task<ActionResult> RatesDownloadData()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetRatesAsync(currentuser);

            if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            {
            }
            else
            {
                //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();
            }
            var ldx = _mapper.Map<IReadOnlyList<Rate>, IReadOnlyList<RateDto>>(ars);

            var ret = await DownloadData<RateDto>("Rate", _fgs, ldx);

            return ret;
        }

        [HttpPost("bulkupload")]
        public async Task<ActionResult> RatesBulkUpload(CancellationToken cancellationToken)
        {
            var currentuser = await GetCurrentUser();

         //   string datefields = "CurrentStartDate,CurrentEndDate,ApplicationEndDate,ApprovalEndDate";

            var fgs = new FormGridService<RateDto>(_fgs.GetUnitOfWork());

            var ret = await BulkUpload<RateDto>(cancellationToken, Request.Form.Files[0], "Rate", fgs, null);
            //  var ret = await BulkUpload<OrganizationDto>(cancellationToken, Request.Form.Files[0], "Organization", _fgs);
            foreach (var v in ret.Headings)
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

            var valsrc = new ImportExcelData<Rate>();
            valsrc.ErrorInFields = ret.ErrorInFields;
            valsrc.Headings = ret.Headings;
            valsrc.DataSource = _mapper.Map<List<RateDto>, List<Rate>>(ret.DataSource);

            valsrc = (ImportExcelData<Rate>)(await _ms.BulkValidateRateAsync(valsrc, currentuser));
            if (!String.IsNullOrEmpty(valsrc.ErrorMessage))
            {
                return BadRequest(new ApiResponse(401, ret.ErrorMessage));
            }
            ret.ErrorInFields = valsrc.ErrorInFields;
            ret.Headings = valsrc.Headings;
            ret.DataSource = _mapper.Map<List<Rate>, List<RateDto>>(valsrc.DataSource);

            SessionManager.Add("RateUpload-" + currentuser.Id.ToString(), System.Text.Json.JsonSerializer.Serialize(ret));

            return Ok(ret);
        }

        [HttpPost("savebulkupload")]
        public async Task<ActionResult> SaveBulkUpload(string UploadCode)
        {
            var currentuser = await GetCurrentUser();

            var up = System.Text.Json.JsonSerializer.Deserialize<ImportExcelData<RateDto>>(SessionManager.Get("OrganizationUpload-" + currentuser.Id.ToString()));

            if (up == null || up.ErrorInFields.Count > 0)
            {
                return BadRequest(new ApiResponse(401, "Cannot Save Data due to Errors !!"));
            }

            var oul = _mapper.Map<IReadOnlyList<RateDto>, IReadOnlyList<Rate>>(up.DataSource);

            var ret = await _ms.SaveUploadRateAsync(oul, currentuser);

            SessionManager.Delete("RateUpload-" + currentuser.Id.ToString());

            return Ok();
        }

        [HttpGet("getgridcols")]
        public async Task<ActionResult> GetGridCols(string FormName)
        {
            var currentuser = await GetCurrentUser();

            if (FormName == "Rate" || FormName == "Rates")
            {
                var gxl = await this.GetFormGridCols<RateDto>(FormName, _fgs);
                return gxl;
            }

            return null;
        }

    }
}
