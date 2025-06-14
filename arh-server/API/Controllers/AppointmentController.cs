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

namespace API.Controllers
{
    public class AppointmentController : BaseWithUserApiController
    {
        IFormGridService<appointmentsDto> _fgs;
        ILogger<AppointmentController> _logger;

        public AppointmentController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<AppointmentController> logger,
                IFormGridService<appointmentsDto> fgs
                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        [HttpGet("getappointmentslist")]
        public async Task<ActionResult<IReadOnlyList<appointmentsDto>>> GetAppointmentsList()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetAppointmentsAsync(currentuser);

            if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            {
                var ldxx = _mapper.Map<IReadOnlyList<appointments>, IReadOnlyList<appointmentsDto>>(ars);
                return Ok(ldxx);
            }

            //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();

            var ldx = _mapper.Map<IReadOnlyList<appointments>, IReadOnlyList<appointmentsDto>>(ars);

            return Ok(ldx);
        }


        [HttpPost("saveappointment")]
        public async Task<ActionResult<appointmentsDto>> SaveAppointment(appointmentsDto appointment)
        {
            if (appointment == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!appointment.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();

            appointments a = null;
            try
            {
                appointment.CreatedOn = "";
                appointment.UCode = null;
                a = _mapper.Map<appointmentsDto, appointments>(appointment);

                a = await _ms.ValidateAppointmentAsync(a, cu);
                if (a.Errors != null && !String.IsNullOrEmpty(a.Errors.errormessage))
                {
                    return BadRequest(new ApiResponse(401, a.Errors.errormessage));
                }

                a = await _ms.SaveAppointmentAsync(a);
                if (a == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }

            return Ok(_mapper.Map<appointments, appointmentsDto>(a));
        }

        [HttpGet("templatedownload")]
        public async Task<ActionResult> AppointmentDownloadTemplate()
        {
            var currentuser = await GetCurrentUser();

            Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
            DefaultValues.Add("IsActive", "Yes");

            var ret = await DownloadTemplate<appointmentsDto>("appointments", _fgs, DefaultValues);

            return ret;
        }

        [HttpGet("downloaddata")]
        public async Task<ActionResult> AppointmentsDownloadData()
        {
            var currentuser = await GetCurrentUser();

            var ars = await _ms.GetAppointmentsAsync(currentuser);

            if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
            {
            }
            else
            {
                //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();
            }
            var ldx = _mapper.Map<IReadOnlyList<appointments>, IReadOnlyList<appointmentsDto>>(ars);

            var ret = await DownloadData<appointmentsDto>("appointments", _fgs, ldx);

            return ret;
        }

        [HttpPost("bulkupload")]
        public async Task<ActionResult> AppointmentsBulkUpload(CancellationToken cancellationToken)
        {
            var currentuser = await GetCurrentUser();

          //  string datefields = "CurrentStartDate,CurrentEndDate,ApplicationEndDate,ApprovalEndDate";

            var fgs = new FormGridService<appointmentsDto>(_fgs.GetUnitOfWork());

            var ret = await BulkUpload<appointmentsDto>(cancellationToken, Request.Form.Files[0], "appointments", fgs, null);
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

            var valsrc = new ImportExcelData<appointments>();
            valsrc.ErrorInFields = ret.ErrorInFields;
            valsrc.Headings = ret.Headings;
            valsrc.DataSource = _mapper.Map<List<appointmentsDto>, List<appointments>>(ret.DataSource);

            valsrc = (ImportExcelData<appointments>)(await _ms.BulkValidateAppointmentAsync(valsrc, currentuser));
            if (!String.IsNullOrEmpty(valsrc.ErrorMessage))
            {
                return BadRequest(new ApiResponse(401, ret.ErrorMessage));
            }
            ret.ErrorInFields = valsrc.ErrorInFields;
            ret.Headings = valsrc.Headings;
            ret.DataSource = _mapper.Map<List<appointments>, List<appointmentsDto>>(valsrc.DataSource);

            SessionManager.Add("AppointmentUpload-" + currentuser.Id.ToString(), System.Text.Json.JsonSerializer.Serialize(ret));

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

            if (FormName == "appointment" || FormName == "appointments")
            {
                var fgs = new FormGridService<appointmentsDto>(_fgs.GetUnitOfWork());
                var gxl = await this.GetFormGridCols<appointmentsDto>(FormName, _fgs);
                return gxl;
            }

            return null;
        }

    }
}

