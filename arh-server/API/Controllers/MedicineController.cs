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
    public class MedicineController : BaseWithUserApiController
    {
        IFormGridService<MedicineDto> _fgs;
        ILogger<MedicineController> _logger;

        public MedicineController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<MedicineController> logger,
                IFormGridService<MedicineDto> fgs
                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
            _fgs = fgs;
        }

        //[HttpGet("getmedicineslist")]
        //public async Task<ActionResult<IReadOnlyList<MedicineDto>>> GetRatesList()
        //{
        //    var currentuser = await GetCurrentUser();

        //    var ars = await _ms.GetMedicinesAsync(currentuser);

        //    //if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
        //    //{
        //    //    var ldxx = _mapper.Map<IReadOnlyList<Rate>, IReadOnlyList<RateDto>>(ars);
        //    //    return Ok(ldxx);
        //    //}

        //    //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();

        //    var ldx = _mapper.Map<IReadOnlyList<Medicine>, IReadOnlyList<MedicineDto>>(ars);

        //    return Ok(ldx);
        //}


        //[HttpPost("savemedicine")]
        //public async Task<ActionResult<MedicineDto>> SaveMedicine(MedicineDto medicine)
        //{
        //    if (medicine == null)
        //    {
        //        return BadRequest(new ApiResponse(401, "No User info received !!"));
        //    }

        //    if (!medicine.IsValidForSave())
        //    {
        //        return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
        //    }

        //    var cu = await GetCurrentUser();

        //    Medicine m = null;
        //    try
        //    {
        //        m = _mapper.Map<MedicineDto, Medicine>(medicine);

        //        m = await _ms.ValidateMedicineAsync(m, cu);
        //        if (m.Errors != null && !String.IsNullOrEmpty(m.Errors.errormessage))
        //        {
        //            return BadRequest(new ApiResponse(401, m.Errors.errormessage));
        //        }

        //        m = await _ms.SaveMedicineAsync(m);
        //        if (m == null)
        //        {
        //            return BadRequest(new ApiResponse(401, "Unable to Save"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
        //    }

        //    return Ok(_mapper.Map<Medicine, MedicineDto>(m));
        //}

        //[HttpGet("templatedownload")]
        //public async Task<ActionResult> MedicinesDownloadTemplate()
        //{
        //    var currentuser = await GetCurrentUser();

        //    Dictionary<string, string> DefaultValues = new Dictionary<string, string>();
        //    DefaultValues.Add("IsActive", "Yes");

        //    var ret = await DownloadTemplate<MedicineDto>("Medicine", _fgs, DefaultValues);
        //    return ret;
        //}

        //[HttpGet("downloaddata")]
        //public async Task<ActionResult> RatesDownloadData()
        //{
        //    var currentuser = await GetCurrentUser();

        //    var ars = await _ms.GetMedicinesAsync(currentuser);

        //    if (currentuser.UserName == "admin" || currentuser.AppRoleCode == "ADMINISTRATOR" || currentuser.AppRoleCode == "SUPER")
        //    {
        //    }
        //    else
        //    {
        //        //	ars = ars.Where(x => {{USER_CONDITION}}).ToList();
        //    }
        //    var ldx = _mapper.Map<IReadOnlyList<Medicine>, IReadOnlyList<MedicineDto>>(ars);

        //    var ret = await DownloadData<MedicineDto>("Medicine", _fgs, ldx);
        //    return ret;
        //}

        //[HttpPost("bulkupload")]
        //public async Task<ActionResult> MedicinesBulkUpload(CancellationToken cancellationToken)
        //{
        //    var currentuser = await GetCurrentUser();

        //    //   string datefields = "CurrentStartDate,CurrentEndDate,ApplicationEndDate,ApprovalEndDate";

        //    var fgs = new FormGridService<MedicineDto>(_fgs.GetUnitOfWork());

        //    var ret = await BulkUpload<MedicineDto>(cancellationToken, Request.Form.Files[0], "Medicine", fgs, null);
        //    //  var ret = await BulkUpload<OrganizationDto>(cancellationToken, Request.Form.Files[0], "Organization", _fgs);
        //    foreach (var v in ret.Headings)
        //    {
        //        v.FieldName = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(v.FieldName);
        //    }

        //    if (ret == null)
        //    {
        //        return BadRequest(new ApiResponse(401, "Error processing file"));
        //    }

        //    if (!String.IsNullOrEmpty(ret.ErrorMessage))
        //    {
        //        return BadRequest(new ApiResponse(401, ret.ErrorMessage));
        //    }

        //    if (ret.DataSource == null || ret.DataSource.Count == 0)
        //    {
        //        return Ok(ret);
        //    }

        //    var valsrc = new ImportExcelData<Medicine>();
        //    valsrc.ErrorInFields = ret.ErrorInFields;
        //    valsrc.Headings = ret.Headings;
        //    valsrc.DataSource = _mapper.Map<List<MedicineDto>, List<Medicine>>(ret.DataSource);

        //    valsrc = (ImportExcelData<Medicine>)(await _ms.BulkValidateMedicineAsync(valsrc, currentuser));
        //    if (!String.IsNullOrEmpty(valsrc.ErrorMessage))
        //    {
        //        return BadRequest(new ApiResponse(401, ret.ErrorMessage));
        //    }
        //    ret.ErrorInFields = valsrc.ErrorInFields;
        //    ret.Headings = valsrc.Headings;
        //    ret.DataSource = _mapper.Map<List<Medicine>, List<MedicineDto>>(valsrc.DataSource);

        //    SessionManager.Add("MedicineUpload-" + currentuser.Id.ToString(), System.Text.Json.JsonSerializer.Serialize(ret));

        //    return Ok(ret);
        //}

        //[HttpPost("savebulkupload")]
        //public async Task<ActionResult> SaveBulkUpload(string UploadCode)
        //{
        //    var currentuser = await GetCurrentUser();

        //    var up = System.Text.Json.JsonSerializer.Deserialize<ImportExcelData<MedicineDto>>(SessionManager.Get("MedicineUpload-" + currentuser.Id.ToString()));

        //    if (up == null || up.ErrorInFields.Count > 0)
        //    {
        //        return BadRequest(new ApiResponse(401, "Cannot Save Data due to Errors !!"));
        //    }

        //    var oul = _mapper.Map<IReadOnlyList<MedicineDto>, IReadOnlyList<Medicine>>(up.DataSource);

        //    var ret = await _ms.SaveUploadMedicineAsync(oul, currentuser);

        //    SessionManager.Delete("MedicineUpload-" + currentuser.Id.ToString());

        //    return Ok();
        //}

        [HttpGet("getgridcols")]
        public async Task<ActionResult> GetGridCols(string FormName)
        {
            var currentuser = await GetCurrentUser();

            if (FormName == "Medicine" || FormName == "Medicines")
            {
                var fgs = new FormGridService<MedicineDto>(_fgs.GetUnitOfWork());
                var columnResult = await this.GetFormGridCols<MedicineDto>(FormName, fgs);
                if (columnResult is not OkObjectResult okColumnResult)
                    return BadRequest("Could not get column metadata.");

                var columns = okColumnResult.Value as IReadOnlyList<FormGridDetailDto>;

                // 2. Get data (patientDto list)
                var medicineEntities = await _fgs.GetUnitOfWork().Repository<Medicine>().ListAllAsync();
                var rows = _mapper.Map<IReadOnlyList<Medicine>, IReadOnlyList<MedicineDto>>(medicineEntities);

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


