using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BaseWithUserApiController : BaseApiController
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly SignInManager<AppUser> _signInManager;
        protected readonly IMastersService _ms;
        protected IMapper _mapper;
        //  protected ILogger _logger;

        public BaseWithUserApiController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms)
        //, ILogger logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _ms = ms;
            _mapper = mapper;
            //  _logger = logger;
        }

        protected async Task<AppUser> GetCurrentUser()
        {
            var userx = await _userManager.FindByEmailAsync(User);

            return userx;

            //        var ux = await _userManager.FindUserFromClaimsPrinciple("admin");

            //        var xx = await _userManager.FindEmailFromClaimsPrinciple(ClaimsPrincipal.Current);

            //        //  if (ux == null)
            //        //  {
            //        //      return null;
            //        //  }

            //        ////  return (await ux.("EDPNo", ux));
            //        //  return ux;
            //        string mobile = User?.Claims
            //.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone ||
            //                     c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone")
            //?.Value;
            //        var userId = User.FindFirst("nameid")?.Value;
            //       // var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier); // gets 'sub' claim from token
            //        return await _userManager.FindUserFromClaimsPrinciple(mobile);
        }
        protected async Task<ActionLog> AddActionLog(ActionLog actlg)
        {
            //var au = await GetCurrentUser();
            var e = await _ms.AddActionLogAsync(actlg);
            return e;
        }

        //protected async Task<AppUser> GetUser()
        //{
        //    var ux = await GetCurrentUser();
        //    var e = await _ms.GetUserFromAppUserAsync(ux);

        //    return e;
        //}

        //protected async Task<UserDto> GetCurrentUserData()
        //{
        //    var ux = await GetCurrentUser();
        //    var e = await _ms.GetUserFromAppUserAsync(ux);
        //    var o = await _ms.GetOrganizationByNameAsync(e.OrganizationName);

        //    var ce = _mapper.Map<User, UserDto>(e);
        //    ce.OrganizationId = o.Id;

        //    return ce;
        //}

        //protected async Task<UserDto> GetCurrentUserData(AppUser ux)
        //{
        //    var e = await _ms.GetUserFromAppUserAsync(ux);
        //    var o = await _ms.GetOrganizationByNameAsync(e.OrganizationName);

        //    var ce = _mapper.Map<User, UserDto>(e);
        //    ce.OrganizationId = o.Id;

        //    return ce;
        //}


        public async Task<ActionResult> GetFormGridCols<T>(string FormName, IFormGridService<T> fgs)
        {
            var gc = await fgs.GetFormGridDetails(FormName, -1);
            gc = gc.OrderBy(x => x.Position).ToList();

            foreach (var v in gc)
            {
                v.FieldName = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(v.FieldName);
            }
            return Ok(_mapper.Map<IReadOnlyList<FormGridDetail>, IReadOnlyList<FormGridDetailDto>>(gc));
        }

        public async Task<ActionResult> RefreshFormGridCols<T>(string FormName, IFormGridService<T> fgs)
        {
            var gc = await fgs.RefreshFormGridDetails(FormName, -1);
            gc = gc.OrderBy(x => x.Position).ToList();

            foreach (var v in gc)
            {
                v.FieldName = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(v.FieldName);
            }
            return Ok(_mapper.Map<IReadOnlyList<FormGridDetail>, IReadOnlyList<FormGridDetailDto>>(gc));
        }

        //  Excel - Download Template, Download Data, Upload Data

        public async Task<ActionResult> DownloadTemplate<T>(string FormName, IFormGridService<T> fgs, Dictionary<string, string> DefaultValues = null)
        {

            var ux = await GetCurrentUser();

            var ret = await fgs.GetTemplateData(FormName, ux, DefaultValues);

            if (ret == null)
            {
                return BadRequest(new ApiResponse(401, "Cannot download Template !!"));
            }
            string contentType = "";

            var fectp = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            fectp.TryGetContentType(FormName + ".xlsx", out contentType);

            return File(ret, contentType, FormName + "Template.xlsx");
        }

        public async Task<ActionResult> DownloadData<T>(string FormName, IFormGridService<T> fgs, IReadOnlyList<T> src)
        {
            var ux = await GetCurrentUser();

            var ret = await fgs.GetFormDataDownload(FormName, ux, src);

            if (ret == null)
            {
                return BadRequest(new ApiResponse(401, "Cannot download data !!"));
            }
            string contentType = "";

            var fectp = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            fectp.TryGetContentType(FormName + ".xlsx", out contentType);

            return File(ret, contentType, FormName + "Data.xlsx");
        }

        public async Task<IImportExcelData<T>> BulkUpload<T>(CancellationToken cancellationToken, IFormFile file, string FormName, IFormGridService<T> fgs, string datefields = null)
        {
            //var file = Request.Form.Files[0];
            string ext = Path.GetExtension(file.FileName);
            if (!ext.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !ext.Equals(".xls", StringComparison.OrdinalIgnoreCase))
            {
                IImportExcelData<T> ret = new ImportExcelData<T>();
                ret.ErrorMessage = "File type not supported, Only XLSX or XLS supported !!";

                //return BadRequest(new ApiResponse(401, "File type not supported, Only XLSX or XLS supported !!"));
                return ret;
            }

            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");

            if (file.Length == 0)
            {
                IImportExcelData<T> ret = new ImportExcelData<T>();
                ret.ErrorMessage = "No Attachment found";

                //return BadRequest(new ApiResponse(500, "No Attachment found"));
                return ret;
            }

            try
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                string PhyFilename = Guid.NewGuid().ToString() + ext;

                var fullPath = Path.Combine(pathToSave, PhyFilename);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var ux = await GetCurrentUser();

                var up = await fgs.ImportExcelData(FormName, ux, fullPath, null, datefields);
                foreach (var v in up.Headings)
                {
                    v.FieldName = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(v.FieldName);
                }

                SessionManager.Add(FormName + "Upload-" + ux.Id.ToString(), System.Text.Json.JsonSerializer.Serialize(up));

                //HttpContext.Session.SetObjectAsJson<IImportExcelData<OfficeUserDto>>("OfficeUserUpload", up);

                return up;
            }
            catch (Exception ex)
            {
                IImportExcelData<T> ret = new ImportExcelData<T>();
                ret.ErrorMessage = $"Internal server error: {ex}";

                //  return StatusCode(500, $"Internal server error: {ex}");
                return ret;
            }
        }

        // public async Task<IImportExcelData<UserSalary>> BulkUploadSalary<UserSalary>(CancellationToken cancellationToken, IFormFile file, string FormName, IFormGridService<UserSalary> fgs, string datefields=null)
        // {
        //     //var file = Request.Form.Files[0];
        //     string ext = Path.GetExtension(file.FileName);
        //     if (!ext.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !ext.Equals(".xls", StringComparison.OrdinalIgnoreCase))
        //     {
        //         IImportExcelData<UserSalary> ret = new ImportExcelData<UserSalary>();
        //         ret.ErrorMessage = "File type not supported, Only XLSX or XLS supported !!";

        //         //return BadRequest(new ApiResponse(401, "File type not supported, Only XLSX or XLS supported !!"));
        //         return ret;
        //     }

        //     var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");

        //     if (file.Length == 0)
        //     {
        //         IImportExcelData<UserSalary> ret = new ImportExcelData<UserSalary>();
        //         ret.ErrorMessage = "No Attachment found";

        //         //return BadRequest(new ApiResponse(500, "No Attachment found"));
        //         return ret;
        //     }

        //     try
        //     {
        //         var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

        //         string PhyFilename = Guid.NewGuid().ToString() + ext;

        //         var fullPath = Path.Combine(pathToSave, PhyFilename);

        //         using (var stream = new FileStream(fullPath, FileMode.Create))
        //         {
        //             await file.CopyToAsync(stream);
        //         }

        //         var ux = await GetCurrentUser();

        //         var up = await fgs.ImportExcelSalaryData(FormName, ux, fullPath, null, datefields);
        //         foreach(var v in up.Headings) 
        //         {
        //             v.FieldName = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(v.FieldName);
        //         }

        //         SessionManager.Add(FormName + "Upload-" + ux.Id.ToString(), System.Text.Json.JsonSerializer.Serialize(up));

        //         //HttpContext.Session.SetObjectAsJson<IImportExcelData<OfficeUserDto>>("OfficeUserUpload", up);

        //         return up;
        //     }
        //     catch (Exception ex)
        //     {
        //         IImportExcelData<UserSalary> ret = new ImportExcelData<UserSalary>();
        //         ret.ErrorMessage = $"Internal server error: {ex}";

        //         //  return StatusCode(500, $"Internal server error: {ex}");
        //         return ret;
        //     }
        // }


        //  ------------------------

    }

}
