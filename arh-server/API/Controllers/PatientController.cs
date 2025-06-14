using API.Dtos;
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
using Infrastructure.Data;

namespace API.Controllers
{
    public class PatientController : BaseWithUserApiController
    {
        IFormGridService<patientDto> _fgs;
        ILogger<PatientController> _logger;

        public PatientController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<PatientController> logger,
                IFormGridService<patientDto> fgs
                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getpatientslist")]
        public async Task<ActionResult<IReadOnlyList<OrganizationDto>>> GetPatientsList()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetPatientsAsync(currentuser);

            if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            {
                var ldxx = _mapper.Map<IReadOnlyList<patient>, IReadOnlyList<patientDto>>(ars);
                return Ok(ldxx);
            }

            //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();

            var ldx = _mapper.Map<IReadOnlyList<patient>, IReadOnlyList<patientDto>>(ars);

            return Ok(ldx);
        }


        [HttpPost("savepatient")]
        public async Task<ActionResult<patientDto>> SavePatient(patientDto patient)
        {
            if (patient == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!patient.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();

            patient o = null;
            try
            {
                patient.CreatedOn = "";
                patient.UCode = null;
                o = _mapper.Map<patientDto, patient>(patient);

                o = await _ms.ValidatePatientAsync(o, cu);
                if (o.Errors != null && !String.IsNullOrEmpty(o.Errors.errormessage))
                {
                    return BadRequest(new ApiResponse(401, o.Errors.errormessage));
                }

                o = await _ms.SavePatientAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }

            return Ok(_mapper.Map<patient, patientDto>(o));
        }

        [HttpGet("templatedownload")]
        public async Task<ActionResult> PatientDownloadTemplate()
        {
            var currentuser = await GetCurrentUser();

            Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
            DefaultValues.Add("IsActive", "Yes");

            var ret = await DownloadTemplate<patientDto>("patient", _fgs, DefaultValues);

            return ret;
        }

        [HttpGet("downloaddata")]
        public async Task<ActionResult> PatientsDownloadData()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetPatientsAsync(currentuser);

            if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            {
            }
            else
            {
                //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();
            }
            var ldx = _mapper.Map<IReadOnlyList<patient>, IReadOnlyList<patientDto>>(ars);

            var ret = await DownloadData<patientDto>("patient", _fgs, ldx);

            return ret;
        }

        [HttpPost("bulkupload")]
        public async Task<ActionResult> PatientsBulkUpload(CancellationToken cancellationToken)
        {
            var currentuser = await GetCurrentUser();

           // string datefields = "CurrentStartDate,CurrentEndDate,ApplicationEndDate,ApprovalEndDate";

            var fgs = new FormGridService<patientDto>(_fgs.GetUnitOfWork());

            var ret = await BulkUpload<patientDto>(cancellationToken, Request.Form.Files[0], "patient", fgs, null);
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

            var valsrc = new ImportExcelData<patient>();
            valsrc.ErrorInFields = ret.ErrorInFields;
            valsrc.Headings = ret.Headings;
            valsrc.DataSource = _mapper.Map<List<patientDto>, List<patient>>(ret.DataSource);

            valsrc = (ImportExcelData<patient>)(await _ms.BulkValidatePatientAsync(valsrc, currentuser));
            if (!String.IsNullOrEmpty(valsrc.ErrorMessage))
            {
                return BadRequest(new ApiResponse(401, ret.ErrorMessage));
            }
            ret.ErrorInFields = valsrc.ErrorInFields;
            ret.Headings = valsrc.Headings;
            ret.DataSource = _mapper.Map<List<patient>, List<patientDto>>(valsrc.DataSource);

            SessionManager.Add("PatientUpload-" + currentuser.Id.ToString(), System.Text.Json.JsonSerializer.Serialize(ret));

            return Ok(ret);
        }

        [HttpPost("savebulkupload")]
        public async Task<ActionResult> SaveBulkUpload(string UploadCode)
        {
            var currentuser = await GetCurrentUser();

            var up = System.Text.Json.JsonSerializer.Deserialize<ImportExcelData<OrganizationDto>>(SessionManager.Get("OrganizationUpload-" + currentuser.Id.ToString()));

            if (up == null || up.ErrorInFields.Count > 0)
            {
                return BadRequest(new ApiResponse(401, "Cannot Save Data due to Errors !!"));
            }

            var oul = _mapper.Map<IReadOnlyList<OrganizationDto>, IReadOnlyList<Organization>>(up.DataSource);

            var ret = await _ms.SaveUploadOrganizationAsync(oul, currentuser);

            SessionManager.Delete("PatientUpload-" + currentuser.Id.ToString());

            return Ok();
        }

        [HttpGet("getgridcols")]
        public async Task<ActionResult> GetGridCols(string FormName)
        {
            var currentuser = await GetCurrentUser();

            if (FormName == "patient" || FormName == "patients")
            {
                var fgs = new FormGridService<patientDto>(_fgs.GetUnitOfWork());
                var columnResult = await this.GetFormGridCols<patientDto>(FormName, fgs);
                if (columnResult is not OkObjectResult okColumnResult)
                    return BadRequest("Could not get column metadata.");

                var columns = okColumnResult.Value as IReadOnlyList<FormGridDetailDto>;

                // 2. Get data (patientDto list)
                var patientEntities = await _fgs.GetUnitOfWork().Repository<patient>().ListAllAsync();
                var rows = _mapper.Map<IReadOnlyList<patient>, IReadOnlyList<patientDto>>(patientEntities);

                // 3. Return combined structure
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
