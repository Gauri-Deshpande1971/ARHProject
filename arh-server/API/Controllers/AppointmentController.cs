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
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Google.Apis.Calendar.v3;
using static API.Controllers.AppointmentController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API.Controllers
{   
    public class AppointmentController : BaseWithUserApiController
    {
        IFormGridService<appointmentsDto> _fgs;
        ILogger<AppointmentController> _logger;
      //  IGoogleCalendarService _googleCalendarService;
        public AppointmentController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<AppointmentController> logger,
                IFormGridService<appointmentsDto> fgs
                                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
         //   _googleCalendarService = googleCalendarService;
        }
        public enum DoctorId
        {
            DrSwapnil = 2,
            DrSandeep =3,
            DrAkshaya = 4
        }
        public static class DoctorCredentialPaths
        {
            public static string GetJsonFilePath(DoctorId doctor)
            {
                return doctor switch
                {
                    DoctorId.DrSwapnil => "C:\\Gauri\\drswapnil-googleapi.json",
                    DoctorId.DrSandeep => "C:\\Gauri\\googleapikey.json",
                    DoctorId.DrAkshaya => "C:\\Gauri\\drakshaya-googleapi.json",
                    _ => throw new ArgumentOutOfRangeException(nameof(doctor), "Invalid doctor")
                };
            }
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
            var patient = await _ms.GetPatientByPatientId(appointment.patient_id);
            var doctor=await _ms.GetdoctorBydoctorId(patient.DoctorId);
            if (patient == null)
            {
                return BadRequest(new ApiResponse(401, "Patient does not exist"));
            }

            appointments a = null;
            try
            {
               // appointment.CreatedOn = "";
            //    appointment.UCode = null;
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
                DoctorId doctorId = (DoctorId)doctor.OfficeUserId;
                var credentialPath = DoctorCredentialPaths.GetJsonFilePath(doctorId);

                var calendarService = new CalendarServiceHelper(credentialPath);
                var summary = doctor.Email;
                var description = $"Follow-up appointment for Patient ID: {patient.RegNo}";
                var start = a.visit_date;
                var end = a.visit_date.AddMinutes(30); // Default 30 min duration

                string eventUrl = await calendarService.AddEventAsync(summary, description, start, end);

                // Optionally store or log the calendar event URL
                Console.WriteLine("Google Event Created: " + eventUrl);
                var smtpHost = "smtp.gmail.com";
                var smtpPort = 587;
                var smtpUser = "deshpande.gauri.b@gmail.com";
                var smtpPass = "fyad hhgl jnlf dgkl";
                var smtpDisplayName = "Your Clinic";
                var smtpSSL = true;

                var subject = "Appointment Confirmation";
                var body = $"Dear {patient.full_name}-{patient.RegNo},<br>Your appointment is confirmed for {DateTime.Now.AddDays(1):f}.<br>Thank you!";
                var attachments = ""; // Pass file paths separated by commas if needed

                var emailer = new EmailerService(); // or use DI
                var result = emailer.SendEmail(
                    gSMTPHost: smtpHost,
                    gSMTPPort: smtpPort,
                    gSMTPAuthentication: true,
                    gSMTPUser: smtpUser,
                    gUserDisplayName: smtpDisplayName,
                    gSMTPPass: smtpPass,
                    gSMTPDomain: "", // optional
                    gSMTPSSL: smtpSSL,
                    sendto: patient.emailId,
                    sendToBCC: "",
                    sendToCC: "",
                    IsSendSeparate: false,
                    Subject: subject,
                    Matter: body,
                    IsBodyHtml: true,
                    Attachements: attachments
                );

                if (result.StartsWith("Error"))
                    return StatusCode(500, result);

                return Ok("Email sent successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }

            return Ok(_mapper.Map<appointments, appointmentsDto>(a));
        }
        [HttpPost("saveappointmentByFileNo")]
        public async Task<ActionResult<appointmentsDto>> SaveAppointmentByFileNo(string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
            {
                return BadRequest(new ApiResponse(401, "No RegNo received!"));
            }

            var cu = await GetCurrentUser();

            // Get patient by RegNo
            var patient = await _ms.GetPatientByRegNoAsync(regNo);
            if (patient == null)
            {
                return BadRequest(new ApiResponse(401, "Patient does not exist"));
            }

            // Create appointment DTO
            var appointmentDto = new appointmentsDto
            {
                patient_id = patient.Id,                       
                CreatedOn = DateTime.UtcNow, // ISO 8601 format if stored as string
                UCode = Guid.Empty,
                visit_date = DateTime.UtcNow,
                IsActive = true,
                IsUpdated = false,
                IsDeleted = false,
                IsExisting = false
            };

            // Map to entity
            var appointment = _mapper.Map<appointments>(appointmentDto);

            try
            {
                // Validate (optional step)
                appointment = await _ms.ValidateAppointmentAsync(appointment, cu);
                if (appointment.Errors != null && !string.IsNullOrEmpty(appointment.Errors.errormessage))
                {
                    return BadRequest(new ApiResponse(401, appointment.Errors.errormessage));
                }
                // Save
                appointment = await _ms.SaveAppointmentAsync(appointment);
                if (appointment == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }

            // Return saved result
            return Ok(_mapper.Map<appointmentsDto>(appointment));
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
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }
            var doctorColors = new Dictionary<int, string>
{
    { 2, "#FFEBEE" },  // Light Red
    { 3, "#E8F5E9" },  // Light Green
    { 4, "#E3F2FD" }   // Light Blue
};

            if (FormName == "Appointment" || FormName == "appointments")
            {
                var fgs = new FormGridService<appointmentsViewDto>(_fgs.GetUnitOfWork());
                var columnResult = await this.GetFormGridCols<appointmentsViewDto>(FormName, fgs);
                if (columnResult is not OkObjectResult okColumnResult)
                    return BadRequest("Could not get column metadata.");

                var columns = okColumnResult.Value as IReadOnlyList<FormGridDetailDto>;

                // 2. Get data (patientDto list)
                var appointmentsEntities = await _fgs.GetUnitOfWork().Repository<appointments>().ListAllAsync();
                var activeAppointments= appointmentsEntities.Where(x=>x.IsActive==true).ToList();
                var patients = await _fgs.GetUnitOfWork().Repository<patient>().ListAllAsync();
                var appUsers = await _userManager.Users.ToListAsync(); // Use this instead

                var result = from appt in activeAppointments
                             join pat in patients on appt.patient_id equals pat.Id into patJoin
                             from pat in patJoin.DefaultIfEmpty()
                             join doc in appUsers on pat.DoctorId equals doc.OfficeUserId into docJoin
                             from doc in docJoin.DefaultIfEmpty()
                             select new appointmentsViewDto
                             {
                                Id= appt.Id,
                                AppointmentDate= appt.visit_date,
                                IsActive= appt.IsActive,
                                PatientFullName = pat?.full_name,
                                PatientRegNo = pat?.RegNo,
                                DoctorId = pat?.DoctorId,
                                AssistanDoctorId=appt.assistantDoctorId,
                                DoctorName = doc?.DisplayName,
                                status = appt.status,
                                CreatedByName="Admin",
                                OfficeUserId = doc.OfficeUserId,
                                RowBackColor = doctorColors.TryGetValue(doc.OfficeUserId, out var color) ? color : null
                             };
                
                return Ok(new
                {
                    Columns = columns,
                    Rows = result
                });

            }
            return null;
        }
    }

}


