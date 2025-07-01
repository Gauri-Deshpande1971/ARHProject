using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using City = Core.Entities.City;
using DocumentFormat.OpenXml.Drawing;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Wordprocessing;
//  using Infrastructure.Data.Migrations;

namespace Infrastructure.Services
{
    public class MastersService : IMastersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<AppUser> _userManager;
        private IConfiguration _config;
        private IPaValidator _pavalidator;
        //private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        //  private  IDataSyncHttpClient _dtsynclient;

        public MastersService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager
                , IPaValidator pavalidator
                , IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _pavalidator = pavalidator;
            _config = config;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<DateTime> GetToday()
        {
            if (!string.IsNullOrEmpty(_config["ConfigureToday"]))
            {
                if (_config["ConfigureToday"] == "Yes")
                {
                    var sys = await _unitOfWork.Repository<SysData>()
                            .GetByNameAsync("FieldName", "TODAY");
                    if (sys != null)
                    {
                        return Convert.ToDateTime(sys.FieldValue);
                    }

                    sys = new SysData();
                    sys.FieldName = "TODAY";
                    sys.FieldValue = string.Format("{0:yyyy-MM-dd}", DateTime.Today);
                    sys.CreatedOn = DateTime.UtcNow;
                    sys.CreatedById = 1;
                    sys.IsActive = true;
                    sys.IsDeleted = false;

                    _unitOfWork.Repository<SysData>().Add(sys);
                    await _unitOfWork.Complete();

                    return Convert.ToDateTime(sys.FieldValue);
                }
            }
            return DateTime.Today;
        }

        public async Task<DateTime> SetToday(string today)
        {
            var sys = await _unitOfWork.Repository<SysData>()
                    .GetByNameAsync("FieldName", "TODAY");
            if (sys == null)
            {
                sys = new SysData();
                sys.FieldName = "TODAY";
                sys.CreatedOn = DateTime.UtcNow;
                sys.CreatedById = 1;
                sys.IsActive = true;
                sys.IsDeleted = false;
            }

            sys.FieldValue = today;

            if (sys.Id == 0)
                _unitOfWork.Repository<SysData>().Add(sys);
            else
                _unitOfWork.Repository<SysData>().Update(sys);

            await _unitOfWork.Complete();

            return Convert.ToDateTime(sys.FieldValue);
        }

        public async Task<int> IncrementFailedCountAsync(OfficeUser ou)
        {
            var r = await _unitOfWork.Repository<OfficeUser>()
                    .GetByIdAsync(ou.Id);

            if (r != null)
            {
                ++r.FailedLoginCount;
                _unitOfWork.Repository<OfficeUser>().Update(r);
                await _unitOfWork.Repository<OfficeUser>().Complete();
                return r.FailedLoginCount;
            }

            return 0;
        }

        public async Task<IReadOnlyList<Core.Entities.Department>> GetDepartmentsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<Core.Entities.Department>().ListAllAsync();
            r = r.OrderBy(x => x.DepartmentName).ToList();

            return r;
        }

        public async Task<Core.Entities.Department> GetDepartmentByNameAsync(string DepartmentName)
        {
            var r = await _unitOfWork.Repository<Core.Entities.Department>()
                    .GetByNameAsync("DepartmentName", DepartmentName);

            return r;
        }
        public async Task<Core.Entities.Department> GetDepartmentByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<Core.Entities.Department>()
                    .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<Core.Entities.Department> ValidateDepartmentAsync(Core.Entities.Department ret, AppUser au, string SubDepartmentNames)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.DepartmentName = ret.DepartmentName.UpperTrim();

            Core.Entities.Department obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.Now;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Core.Entities.Department>()
                        .GetEntityWithSpec(new BaseSpecification<Core.Entities.Department>(x => x.DepartmentName == ret.DepartmentName
                            && x.ParId == 0
                            && x.OrganizationNameInclude == ret.OrganizationNameInclude)
                        );
                if (dup != null)
                {
                    ret.AddErrorMessage("Department already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<Core.Entities.Department>()
                        .GetEntityWithSpec(new BaseSpecification<Core.Entities.Department>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown Department for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Core.Entities.Department>()
                        .GetEntityWithSpec(new BaseSpecification<Core.Entities.Department>(x => x.DepartmentName == ret.DepartmentName
                            && x.OrganizationNameInclude == ret.OrganizationNameInclude
                            && x.ParId == 0
                            && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("Department already exists !!");
                    return ret;
                }
            }
            obj.DepartmentName = ret.DepartmentName;
            obj.OrganizationNameExclude = ret.OrganizationNameExclude;
            obj.OrganizationNameInclude = ret.OrganizationNameInclude;
            obj.IsActive = ret.IsActive;

            return obj;
        }

        public async Task<IImportExcelData<Core.Entities.Department>> BulkValidateDepartmentAsync(IImportExcelData<Core.Entities.Department> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            var dupsrc = await _unitOfWork.Repository<Core.Entities.Department>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].DepartmentName = ret.DataSource[row].DepartmentName.UpperTrim();
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.Now;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.DepartmentName == ret.DataSource[row].DepartmentName).Any())
                {
                    ret.AddErrorMessage("departmentName", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.DepartmentName == ret.DataSource[row].DepartmentName).Count() > 1)
                {
                    ret.AddErrorMessage("departmentName", "Duplicate in List", row);
                }
            }
            return ret;
        }
        public async Task<Core.Entities.Department> SaveDepartmentAsync(Core.Entities.Department ret, string SubDepartmentNames)
        {
            var ssts = _config["SyncServerType"];
            bool isNew = false;
            if (ret.Id == 0)
            {
                isNew = true;
                _unitOfWork.Repository<Core.Entities.Department>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<Core.Entities.Department>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<Core.Entities.Department>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    //  Save Sub-Departments
                    var sds = (await _unitOfWork.Repository<Core.Entities.Department>()
                        .ListAsync(new BaseSpecification<Core.Entities.Department>(x => x.ParId == ret.Id)))
                        .ToList();
                    foreach (var sdx in sds)
                    {
                        sdx.IsDeleted = true;
                    }
                    var resx = await _unitOfWork.Repository<Core.Entities.Department>().Complete();

                    if (!String.IsNullOrEmpty(SubDepartmentNames))
                    {
                        foreach (var sdx in SubDepartmentNames.Split(','))
                        {
                            var d = new Core.Entities.Department();
                            d.DepartmentName = sdx;
                            d.ParId = ret.Id;
                            d.CreatedOn = DateTime.Now;
                            d.CreatedByName = ret.CreatedByName;
                            d.IsActive = true;
                            d.IsDeleted = false;
                            d.SequenceNo = 0;
                            d.UCode = Guid.NewGuid();

                            _unitOfWork.Repository<Core.Entities.Department>().Add(d);
                        }
                        await _unitOfWork.Repository<Core.Entities.Department>().Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<bool> SaveUploadDepartmentAsync(IReadOnlyList<Core.Entities.Department> Departments, AppUser au)
        {
            foreach (var t in Departments)
            {
                var tx = await SaveDepartmentAsync(t, (string)null);
            }

            return true;
        }
        public async Task<Boolean> LoginSucceeded(OfficeUser ou)
        {
            var r = await _unitOfWork.Repository<OfficeUser>()
                    .GetByIdAsync(ou.Id);

            if (r != null)
            {
                r.FailedLoginCount = 0;
                r.LastLogin = DateTime.UtcNow;
                //   r.ExtraValue1 = "s";
                //  r.ExtraValue2 = "d";
                _unitOfWork.Repository<OfficeUser>().Update(r);
                await _unitOfWork.Repository<OfficeUser>().Complete();
                return true;
            }

            return false;
        }

        public async Task<AppUser> GetLoginOtpAsync(AppUser user, string otpType)
        {
            Random random = new Random();
            int OTP = random.Next(10000, 99999); //  Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 6);
            user.OtpNo = OTP.ToString();
            //  user.OtpNo = "12345";
            user.OtpCode = Guid.NewGuid().ToString();
            user.OtpValidUpto = DateTime.UtcNow.AddMinutes(5);
            user.OtpType = otpType;
            // if (string.IsNullOrEmpty(user.MobileNo))
            //     user.OtpType = "Email";
            // else
            //     user.OtpType = "Mobile";

            await _userManager.UpdateAsync(user);

            return user;
        }

        public async Task<bool> SaveImeiNo(OfficeUser ou)
        {
            var oux = await _unitOfWork.Repository<OfficeUser>()
                .GetByIdAsync(ou.Id);

            if (oux != null)
            {
                oux.IMEINo = ou.IMEINo;

                _unitOfWork.Repository<OfficeUser>().Update(oux);

                await _unitOfWork.Repository<OfficeUser>().Complete();

            }

            return true;
        }
        public async Task<SysData> GetGlobalOTP(string LoginId)
        {
            var emp = await _unitOfWork.Repository<OfficeUser>()
                .GetByNameAsync("LoginId", LoginId);

            var sys = await _unitOfWork.Repository<SysData>()
                    .GetByNameAsync("FieldName", "GLOBAL_OTP");
            if (sys == null || string.IsNullOrEmpty(sys.FieldValue) || sys.CreatedOn < DateTime.Today)
            {
                //  If Allowed Employees found, then allow OTP Generation
                var sysalwd = await _unitOfWork.Repository<SysData>()
                        .GetByNameAsync("FieldName", "ALLOWED_GLOBAL_OTP");
                if (sysalwd != null && sysalwd.FieldValue.Contains(emp.OfficeUserCode))
                {
                    if (sys == null)
                    {
                        sys = new SysData();
                    }
                    sys.FieldName = "GLOBAL_OTP";
                    sys.FieldValue = "OTP";

                    return sys;
                }
                return null;
            }

            return sys;
        }

        public async Task<SysData> SaveGlobalOTP(AppUser au)
        {
            var sys = await _unitOfWork.Repository<SysData>()
                    .GetByNameAsync("FieldName", "GLOBAL_OTP");
            if (sys == null || string.IsNullOrEmpty(sys.FieldValue))
            {
                sys = new SysData();
            }

            //    sys.CreatedById = au.OfficeUserId;
            sys.CreatedByName = au.UserName + "-" + au.DisplayName;
            sys.CreatedOn = DateTime.UtcNow;
            sys.FieldName = "GLOBAL_OTP";
            sys.IsActive = true;
            sys.IsDeleted = false;
            sys.SequenceNo = 0;
            sys.UCode = Guid.NewGuid();

            Random random = new Random();
            int OTP = random.Next(10000, 99999); //  Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 6);
            sys.FieldValue = OTP.ToString();

            if (sys.Id == 0)
                _unitOfWork.Repository<SysData>().Add(sys);
            else
                _unitOfWork.Repository<SysData>().Update(sys);

            await _unitOfWork.Repository<SysData>().Complete();

            return sys;
        }

        public async Task<SysData> SaveGlobalOTP(String OfficeUserCode)
        {
            if (string.IsNullOrEmpty(OfficeUserCode))
                return new SysData();

            var emp = await _unitOfWork.Repository<OfficeUser>()
                .GetByNameAsync("OfficeUserCode", OfficeUserCode);      //  LoginId

            if (emp == null)
                return new SysData();

            var sysalwd = await _unitOfWork.Repository<SysData>()
                    .GetByNameAsync("FieldName", "ALLOWED_GLOBAL_OTP");

            if (sysalwd == null || string.IsNullOrEmpty(sysalwd.FieldValue) || !sysalwd.FieldValue.Contains(emp.OfficeUserCode))
                return new SysData();

            var sys = await _unitOfWork.Repository<SysData>()
                    .GetByNameAsync("FieldName", "GLOBAL_OTP");
            if (sys == null || string.IsNullOrEmpty(sys.FieldValue))
            {
                sys = new SysData();
            }

            sys.CreatedById = emp.Id;
            sys.CreatedByName = emp.LoginId + "-" + emp.OfficeUserName;
            sys.CreatedOn = DateTime.UtcNow;
            sys.FieldName = "GLOBAL_OTP";
            sys.IsActive = true;
            sys.IsDeleted = false;
            sys.SequenceNo = 0;
            sys.UCode = Guid.NewGuid();

            Random random = new Random();
            int OTP = random.Next(10000, 99999); //  Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 6);
            sys.FieldValue = OTP.ToString();

            if (sys.Id == 0)
                _unitOfWork.Repository<SysData>().Add(sys);
            else
                _unitOfWork.Repository<SysData>().Update(sys);

            await _unitOfWork.Repository<SysData>().Complete();

            await AddActionLogAsync("SaveGlobalOTP", "Success", "SysData", sys.FieldValue);

            return sys;
        }

        public async Task<IReadOnlyList<NavmenuOfUser>> GetNavmenuOfUserAsync(string appUserId)
        {
            var appUser = _userManager.Users.FirstOrDefault(x => x.Id == appUserId);

            var nms = await _unitOfWork.Repository<NavMenu>().ListAllAsync();

            var ums = await _unitOfWork.Repository<UserNavMenu>()
                .ListAsync(new BaseSpecification<UserNavMenu>(x => x.AppUserId == appUserId && !x.IsDeleted));

            List<int> nmids;

            if (ums == null || ums.Count == 0)
            {
                nmids = nms
                    .Where(x => !string.IsNullOrEmpty(x.AppRoleCode) && x.AppRoleCode.Contains(appUser.AppRoleCode))
                    .Select(x => x.Id)
                    .ToList();
            }
            else
            {
                nmids = ums.Select(x => x.NavMenuId).ToList();
            }

            var menuDict = nms.ToDictionary(x => x.Id, x => x);

            var nous = new List<NavmenuOfUser>();

            // Get parent menus either directly selected or whose children are selected
            var topMenus = nms
                .Where(x => x.ParId == 0 && (nmids.Contains(x.Id) || nms.Any(c => c.ParId == x.Id && nmids.Contains(c.Id))))
                .OrderBy(x => x.SequenceNo)
                .ToList();

            foreach (var parent in topMenus)
            {
                var nou = new NavmenuOfUser
                {
                    Id = parent.Id,
                    NavLink = parent.NavLink,
                    NavMenuName = parent.NavMenuName,
                    IconClass = parent.IconClass,
                    Description = parent.Description,
                    Submenus = new List<NavmenuOfUser>()
                };

                // Add child submenus if present in allowed ids
                var submenus = nms
                    .Where(x => x.ParId == parent.Id && nmids.Contains(x.Id))
                    .OrderBy(x => x.SequenceNo)
                    .ToList();

                foreach (var sub in submenus)
                {
                    nou.Submenus.Add(new NavmenuOfUser
                    {
                        Id = sub.Id,
                        NavLink = sub.NavLink,
                        NavMenuName = sub.NavMenuName,
                        IconClass = sub.IconClass,
                        Description = sub.Description
                    });
                }

                nous.Add(nou);
            }

            return nous;
        }
        public async Task<OfficeUser> GetEmployeeFromAppUserAsync(AppUser user)
        {
            var emp = await _unitOfWork.Repository<OfficeUser>()
                        .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.AppUserId == user.Id));

            return emp;
        }
        public async Task<IReadOnlyList<OfficeUser>> GetEmployeeFromAppRoleCodeAsync(string AppRoleCode, AppUser user)
        {
            var emplist = await _unitOfWork.Repository<OfficeUser>()
                        .ListAsync(new BaseSpecification<OfficeUser>(x => x.AppRoleCode == AppRoleCode));

            return emplist;
        }

        public async Task<IReadOnlyList<OfficeUser>> GetCompanyAdminEmployeesAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<OfficeUser>().ListAllAsync();

            if (appUser.AppRoleCode == "SUPER" || appUser.AppRoleCode == "SUPERUSER" || appUser.AppRoleCode == "ADMINISTRATOR" || appUser.AppRoleCode == "ADMIN")
            {
                r = r.Where(x => x.AppRoleCode == "COMPANYADN").ToList();
            }
            else
            {
                var emp = await _unitOfWork.Repository<OfficeUser>()
                        .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.AppUserId == appUser.Id));

                r = r.Where(x => x.AppRoleCode == "COMPANYADN" && x.OrganizationName == emp.OrganizationName).ToList();
            }

            return r;
        }

        public async Task<IReadOnlyList<NavmenuOfSuperUser>> GetNavmenuOfSuperUserAsync(string appUserId)
        {
            var nms = await _unitOfWork.Repository<NavMenu>().ListAllAsync();

            var emp = await _unitOfWork.Repository<OfficeUser>()
                .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.AppUserId == appUserId && x.IsDeleted == false));

            // var nmids = ums.Select(x => x.NavMenuId).ToList();

            if (emp.AppRoleCode != "SUPER" && emp.AppRoleCode != "ADMINISTRATOR")
            {
                nms = nms.Where(x => x.NavMenuName != "IT SECURITY").ToList();
            }

            List<NavmenuOfSuperUser> nous = new List<NavmenuOfSuperUser>();
            foreach (var n in nms.Where(x => x.ParId == 0).ToList())
            {
                NavmenuOfSuperUser nou = new NavmenuOfSuperUser()
                {
                    Id = n.Id,
                    NavLink = n.NavLink,
                    NavMenuName = n.NavMenuName,
                    IconClass = n.IconClass,
                    Description = n.Description,
                    AppRoleCode = n.AppRoleCode,
                    UCode = n.UCode.ToString()
                };

                nou.Submenus = new List<NavmenuOfSuperUser>();
                foreach (var nx in nms.Where(x => x.ParId == n.Id).ToList())
                {
                    NavmenuOfSuperUser nox = new NavmenuOfSuperUser()
                    {
                        Id = nx.Id,
                        NavLink = nx.NavLink,
                        NavMenuName = nx.NavMenuName,
                        IconClass = nx.IconClass,
                        Description = nx.Description,
                        AppRoleCode = nx.AppRoleCode,
                        UCode = nx.UCode.ToString()
                    };

                    nou.Submenus.Add(nox);
                }

                nous.Add(nou);
            }

            return nous;
        }
        public async Task<IReadOnlyList<NavmenuOfSuperUser>> SaveNavmenuOfSuperUserAsync(AppUser currentuser, List<NavmenuOfSuperUser> navsuperuser)
        {
            var nms = await _unitOfWork.Repository<NavMenu>().ListAllAsync();
            bool changesdone = false;

            foreach (var i in navsuperuser)
            {
                if (String.IsNullOrEmpty(i.AppRoleCode) && i.Submenus != null && i.Submenus.Count > 0)
                {
                    foreach (var b in i.Submenus)
                    {
                        if (b.AppRoleCode == null)
                        {
                            var nv = nms.Where(x => x.AppRoleCode != null && x.Id == b.Id && x.IsDeleted == false).FirstOrDefault();

                            if (nv != null)
                            {
                                var rolenameb = nv.AppRoleCode.Replace(",", "");    //  remove the extra comma

                                var usernavb = await _unitOfWork.Repository<NavMenu>()
                                .GetEntityWithSpec(new BaseSpecification<NavMenu>(x => x.Id == b.Id && x.IsDeleted == false));

                                usernavb.AppRoleCode = b.AppRoleCode;

                                _unitOfWork.Repository<NavMenu>().Update(usernavb);
                                changesdone = true;
                            }
                        }
                        else
                        {
                            var navs = nms.Where(x => x.AppRoleCode != b.AppRoleCode && x.Id == b.Id && x.IsDeleted == false).FirstOrDefault();

                            if (navs != null)
                            {
                                // Admin, Manager,
                                // Admin, Manager, User, 
                                var usernavx = await _unitOfWork.Repository<NavMenu>()
                                .GetEntityWithSpec(new BaseSpecification<NavMenu>(x => x.Id == b.Id && x.IsDeleted == false));

                                usernavx.AppRoleCode = b.AppRoleCode;

                                _unitOfWork.Repository<NavMenu>().Update(usernavx);
                                changesdone = true;
                            }
                        }
                    }
                }
                else
                {
                    var navsxx = nms.Where(x => x.AppRoleCode != i.AppRoleCode && x.Id == i.Id && x.IsDeleted == false).FirstOrDefault();

                    if (navsxx != null)
                    {
                        // Admin, Manager,
                        // Admin, Manager, User, 
                        var usernavyy = await _unitOfWork.Repository<NavMenu>()
                            .GetEntityWithSpec(new BaseSpecification<NavMenu>(x => x.Id == i.Id && x.IsDeleted == false));

                        usernavyy.AppRoleCode = i.AppRoleCode;
                        _unitOfWork.Repository<NavMenu>().Update(usernavyy);
                        changesdone = true;
                    }
                }
            }

            if (changesdone)
            {
                var result = await _unitOfWork.Complete();
                if (result <= 0)
                {
                    return null;
                }
            }

            return navsuperuser;
        }

        public async Task<IReadOnlyList<NavmenuOfSuperUser>> SaveNavmenuOfUserAsync(AppUser cu, string OfficeUserCode, string AppRoleCode, List<NavmenuOfSuperUser> navsuperuser)
        {
            OfficeUser obj = await _unitOfWork.Repository<OfficeUser>()
                    .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.OfficeUserCode == OfficeUserCode && x.AppUserId != null && x.AppRoleCode == AppRoleCode && x.IsDeleted == false));

            return navsuperuser;
        }

        //  -----------------------
        //  Auto Generated
        //  -----------------------
        #region Auto Generated
        public async Task<IReadOnlyList<AppRole>> GetAppRolesAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<AppRole>().ListAllAsync();

            return r;
        }

        public async Task<AppRole> GetAppRoleByNameAsync(string AppRoleName)
        {
            var r = await _unitOfWork.Repository<AppRole>()
                     .GetByNameAsync("AppRoleName", AppRoleName);

            return r;
        }

        public async Task<AppRole> GetAppRoleByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<AppRole>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<Attachment>> GetAttachmentsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<Attachment>().ListAllAsync();

            return r;
        }

        public async Task<Attachment> GetAttachmentByNameAsync(string AttachmentName)
        {
            var r = await _unitOfWork.Repository<Attachment>()
                     .GetByNameAsync("AttachmentName", AttachmentName);

            return r;
        }

        public async Task<Attachment> GetAttachmentByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<Attachment>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<City>> GetCitiesAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<City>().ListAllAsync();
            r = r.OrderBy(x => x.CityName).ToList();

            return r;
        }

        public async Task<City> GetCityByNameAsync(string CityName)
        {
            var r = await _unitOfWork.Repository<City>()
                     .GetByNameAsync("CityName", CityName);

            return r;
        }

        public async Task<City> GetCityByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<City>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<Country>> GetCountriesAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<Country>().ListAllAsync();
            r = r.OrderBy(x => x.CountryName).ToList();

            return r;
        }

        public async Task<Country> GetCountryByNameAsync(string CountryName)
        {
            var r = await _unitOfWork.Repository<Country>()
                     .GetByNameAsync("CountryName", CountryName);

            return r;
        }

        public async Task<Country> GetCountryByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<Country>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }
        public async Task<IReadOnlyList<OfficeUser>> GetOfficeUsersAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<OfficeUser>().ListAllAsync();

            if (appUser != null && appUser.AppRoleCode == "COMPANYADN")     // || appUser.AppRoleCode == "MANAGER")
            {
                var sd = await _unitOfWork.Repository<OfficeUser>()
                .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.AppUserId == appUser.Id));
                if (sd != null)
                {
                    r = r.Where(x => x.OrganizationName == sd.OrganizationName &&
                    (x.AppRoleCode != "ADMINISTRATOR" && x.AppRoleCode != "COMPANYADN" && x.AppRoleCode != "SUPER")).ToList();
                }
            }
            else
            {
                r = r.Where(x => x.AppRoleCode != "ADMINISTRATOR" && x.AppRoleCode != "COMPANYADN" && x.AppRoleCode != "SUPER").ToList();
            }

            if (r != null)
                r = r.OrderBy(x => x.OfficeUserCode).ToList();

            return r;
        }

        public async Task<IReadOnlyList<OfficeUser>> SearchCompanyAdminEmployeesAsync(string searchterm)
        {
            var sd = await _unitOfWork.Repository<OfficeUser>()
                .ListAsync(new BaseSpecification<OfficeUser>(x => (x.OfficeUserName + "-" + x.OfficeUserCode).Contains(searchterm)
                        && x.IsActive == true
                        && (x.AppRoleCode != "ADMINISTRATOR" || x.AppRoleCode != "SUPER")));

            if (sd != null)
                sd = sd.OrderBy(x => x.OfficeUserCode).ToList();

            return sd;
        }
        public async Task<IReadOnlyList<OfficeUser>> SearchOfficeUsersBySearchAsync(string searchterm)
        {
            var sd = await _unitOfWork.Repository<OfficeUser>()
                .ListAsync(new BaseSpecification<OfficeUser>(x => x.OfficeUserCode == searchterm && (x.AppRoleCode != "ADMINISTRATOR"
                || x.AppRoleCode != "SUPER")));

            if (sd != null)
                sd = sd.OrderBy(x => x.OfficeUserCode).ToList();

            return sd;
        }

        public async Task<OfficeUser> GetOfficeUserByNameAsync(string OfficeUserName)
        {
            var r = await _unitOfWork.Repository<OfficeUser>()
                     .GetByNameAsync("OfficeUserName", OfficeUserName);

            return r;
        }

        public async Task<OfficeUser> GetOfficeUserByCodeAsync(string OfficeUserCode)
        {
            var r = await _unitOfWork.Repository<OfficeUser>()
                     .GetByNameAsync("OfficeUserCode", OfficeUserCode);

            return r;
        }
        public async Task<OfficeUser> GetCompanyAdminOfficeUserByCodeAsync(string OfficeUserCode)
        {
            var r = await _unitOfWork.Repository<OfficeUser>()
              .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.OfficeUserCode == OfficeUserCode && x.AppRoleCode == "COMPANYADN"));

            return r;
        }

        public async Task<OfficeUser> GetOfficeUserOfOrganizationAsync(string OrganizationName, string OfficeUserCode, string OfficeUserName, AppUser au)
        {
            if (au == null)
                return null;

            if (au.AppRoleCode == "COMPANYADN" || au.AppRoleCode == "USER")
            {
                var r = await _unitOfWork.Repository<OfficeUser>()
                .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.OfficeUserCode == OfficeUserCode
                        && x.OrganizationName == OrganizationName
                        && x.OfficeUserName == OfficeUserName));

                return r;
            }

            return null;
        }

        public async Task<IReadOnlyList<FormGridHeader>> GetFormGridHeadersAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<FormGridHeader>().ListAllAsync();

            return r;
        }

        public async Task<FormGridHeader> GetFormGridHeaderByNameAsync(string FormGridHeaderName)
        {
            var r = await _unitOfWork.Repository<FormGridHeader>()
                    .GetByNameAsync("FormGridHeaderName", FormGridHeaderName);

            return r;
        }

        public async Task<FormGridHeader> GetFormGridHeaderByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<FormGridHeader>()
                    .GetByNameAsync("UCode", Ucode);

            return r;
        }
        public async Task<IReadOnlyList<MailConfig>> GetMailConfigsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<MailConfig>().ListAllAsync();

            return r;
        }

        public async Task<MailConfig> GetMailConfigByNameAsync(string MailConfigName)
        {
            var r = await _unitOfWork.Repository<MailConfig>()
                     .GetByNameAsync("MailConfigName", MailConfigName);

            return r;
        }
        public async Task<MailConfig> GetMailConfigByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<MailConfig>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<MailLog>> GetMailLogsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<MailLog>().ListAllAsync();

            return r;
        }

        public async Task<MailLog> GetMailLogByNameAsync(string MailLogName)
        {
            var r = await _unitOfWork.Repository<MailLog>()
                     .GetByNameAsync("MailLogName", MailLogName);

            return r;
        }

        public async Task<MailLog> GetMailLogByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<MailLog>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<NavMenu>> GetNavMenusAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<NavMenu>().ListAllAsync();

            return r;
        }

        public async Task<NavMenu> GetNavMenuByNameAsync(string NavMenuName)
        {
            var r = await _unitOfWork.Repository<NavMenu>()
                     .GetByNameAsync("NavMenuName", NavMenuName);

            return r;
        }

        public async Task<NavMenu> GetNavMenuByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<NavMenu>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<Organization>> GetOrganizationsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<Organization>().ListAllAsync();

            // if (appUser.AppRoleCode == "COMPANYADN" || appUser.AppRoleCode == "MANAGER")
            if (appUser.AppRoleCode == "COMPANYADN")
            {
                var sd = await _unitOfWork.Repository<OfficeUser>()
                .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.AppUserId == appUser.Id));

                if (sd != null)
                {
                    r = r.Where(x => x.OrganizationName == sd.OrganizationName).ToList();
                }
            }
            else if (appUser.AppRoleCode == "ADMINISTRATOR" || appUser.AppRoleCode == "SUPER" || appUser.AppRoleCode == "MANAGER")
            {
                r = r.ToList();
            }
            r = r.OrderBy(x => x.OrganizationName).ToList();

            return r;
        }

        public async Task<Organization> GetOrganizationByNameAsync(string OrganizationName)
        {
            var r = await _unitOfWork.Repository<Organization>()
                     .GetByNameAsync("OrganizationName", OrganizationName);

            return r;
        }

        public async Task<Organization> GetOrganizationByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<Organization>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<State>> GetStatesAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<State>().ListAllAsync();
            r = r.OrderBy(x => x.StateName).ToList();

            return r;
        }

        public async Task<State> GetStateByNameAsync(string StateName)
        {
            var r = await _unitOfWork.Repository<State>()
                     .GetByNameAsync("StateName", StateName);

            return r;
        }

        public async Task<State> GetStateByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<State>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<SysData>> GetSysDatasAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<SysData>().ListAllAsync();

            return r;
        }
        public async Task<SysData> GetSysDataByNameAsync(string SysDataName)
        {
            var r = await _unitOfWork.Repository<SysData>()
                     .GetByNameAsync("FieldName", SysDataName);

            return r;
        }

        public async Task<SysData> GetSysDataByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<SysData>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<UserNavMenu>> GetUserNavMenusAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<UserNavMenu>().ListAllAsync();

            return r;
        }

        public async Task<UserNavMenu> GetUserNavMenuByNameAsync(string UserNavMenuName)
        {
            var r = await _unitOfWork.Repository<UserNavMenu>()
                     .GetByNameAsync("UserNavMenuName", UserNavMenuName);

            return r;
        }

        public async Task<UserNavMenu> GetUserNavMenuByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<UserNavMenu>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<IReadOnlyList<FormGridDetail>> GetFormGridDetailsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<FormGridDetail>().ListAllAsync();

            return r;
        }

        public async Task<FormGridDetail> GetFormGridDetailByNameAsync(string FormGridDetailName)
        {
            var r = await _unitOfWork.Repository<FormGridDetail>()
                     .GetByNameAsync("FormGridDetailName", FormGridDetailName);

            return r;
        }

        public async Task<FormGridDetail> GetFormGridDetailByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<FormGridDetail>()
                     .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<OfficeUser> GetCompanyAdminEmployeeAsync(string OfficeUserName)
        {
            var sd = await _unitOfWork.Repository<OfficeUser>()
                    .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.OfficeUserCode == OfficeUserName && (x.AppRoleCode != "ADMINISTRATOR"
         || x.AppRoleCode != "SUPER")));

            return sd;
        }

        #endregion

        #region Attachments
        public async Task<IReadOnlyList<Attachment>> GetAttachmentListAsync(string EntityName, string EntityFieldName, string EntityKeyValue)
        {
            var a = await _unitOfWork.Repository<Attachment>()
                    .ListAsync(new BaseSpecification<Attachment>(x =>
                        x.EntityName == EntityName &&
                        x.EntityKeyValue == EntityKeyValue &&
                        x.EntityFieldName == EntityFieldName
                    ));

            return a;
        }

        public async Task<Attachment> GetAttachmentAsync(string EntityName, string EntityFieldName, string EntityKeyValue, string FileType)
        {
            var a = await _unitOfWork.Repository<Attachment>()
                    .GetEntityWithSpec(new BaseSpecification<Attachment>(x =>
                        x.EntityName == EntityName &&
                        x.EntityKeyValue == EntityKeyValue &&
                        x.EntityFieldName == EntityFieldName &&
                        x.FileType == FileType
                    ));

            return a;
        }

        public async Task<bool> SaveAttachFile(Attachment attachment)
        {

            if (attachment.Id == 0)
            {
                _unitOfWork.Repository<Attachment>().Add(attachment);
            }
            else
            {
                _unitOfWork.Repository<Attachment>().Update(attachment);
            }

            var res = await _unitOfWork.Repository<Attachment>().Complete();
            if (res <= 0)
            {
                return false;
            }

            return true;
        }


        #endregion

        #region Mobile SMS Service
        public async Task<AppUser> SendOtpOnMobile(AppUser user)
        {
            return user;
        }

        #endregion

        #region Mail Service
        public async Task<AppUser> SendOtpByMail(AppUser user, string ActionName)
        {
            //  OTP is generated
            // Random random = new Random();
            // int OTP = random.Next(100000, 999999); //  Guid.NewGuid().ToString().Replace("-", ").ToUpper().Substring(0, 6);

            // user.OtpCode = Guid.NewGuid().ToString();
            // user.OtpNo  = OTP.ToString();
            // user.OtpValidUpto = DateTime.UtcNow.AddMinutes(10);
            // await _userManager.UpdateAsync(user);

            var mc = await _unitOfWork.Repository<MailConfig>()
                    .GetByNameAsync("MailAction", ActionName);
            if (mc == null)
            {
                return user;
            }

            var ml = new MailLog();
            var content = mc.MailContent;
            content = content.Replace("{{" + ActionName + "}}", user.OtpNo);

            //    ml.CreatedById = user.OfficeUserId;
            ml.CreatedById = 1;
            ml.CreatedByName = user.UserName + "-" + user.DisplayName;
            ml.CopyFromConfig(mc, content, user.Email);
            ml.MailSubject = ml.MailSubject.Replace("{{OfficeUserCode}}", user.OfficeUserCode);

            _unitOfWork.Repository<MailLog>().Add(ml);
            await _unitOfWork.Repository<MailLog>().Complete();

            return user;

        }

        public async Task<AppUser> ChangePassword(AppUser currentUser, string NewPassword, bool savetoextdatasync = false)
        {
            var ssts = _config["SyncServerType"];
            var exturl = _config["ExternalServerUrl"];
            bool exec = false;

            string newpass = NewPassword;

            var ui = currentUser;

            if (ui == null)
            {
                return null;
            }

            if (savetoextdatasync)
            {
                if (ssts == "External")
                {
                    var appuser = await _userManager.FindByIdAsync(ui.Id);

                    if (appuser != null)
                    {
                        //appuser = new AppUser();


                        appuser.ChangePassword = ui.ChangePassword;
                        appuser.LastPasswordChange = ui.LastPasswordChange;
                        appuser.OtpCode = ui.OtpCode;
                        appuser.OtpNo = ui.OtpNo;
                        appuser.OtpValidUpto = ui.OtpValidUpto;
                        appuser.OtpType = ui.OtpType;

                        appuser.PasswordHash = ui.PasswordHash;
                        appuser.SecurityStamp = ui.SecurityStamp;
                        appuser.ConcurrencyStamp = ui.ConcurrencyStamp;

                        appuser.TwoFactorEnabled = ui.TwoFactorEnabled;
                        appuser.LockoutEnd = ui.LockoutEnd;
                        appuser.LockoutEnabled = ui.LockoutEnabled;
                        appuser.AccessFailedCount = ui.AccessFailedCount;


                        await _userManager.UpdateAsync(appuser);
                    }

                    return ui;
                }
            }
            else
            {
                var pres = await _pavalidator.ValidatePasswordAsync(ui, newpass);
                if (!pres)
                {
                    return null;
                }

                string token = "";
                try
                {
                    token = await _userManager.GeneratePasswordResetTokenAsync(ui);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                }
                var rres = await _userManager.ResetPasswordAsync(ui, token, NewPassword);
                if (rres.Succeeded)
                {
                    ui.ChangePassword = false;
                    ui.LastPasswordChange = DateTime.UtcNow;
                    await _userManager.UpdateAsync(ui);

                    await ChangePasswordMail(currentUser, NewPassword);

                    return ui;
                }
            }

            return null;
        }

        public async Task<bool> PasswordResetSuccess(AppUser user)
        {
            var mc = await _unitOfWork.Repository<MailConfig>()
                    .GetByNameAsync("MailAction", "PASSWORD_RESET_SUCCESS");
            if (mc == null)
            {
                return false;
            }
            var ml = new MailLog();
            var content = mc.MailContent;

            ml.CreatedById = user.OfficeUserId;
            ml.CreatedByName = user.UserName + "-" + user.DisplayName;
            ml.CopyFromConfig(mc, content, user.Email);
            ml.MailSubject = ml.MailSubject.Replace("{{OfficeUserCode}}", user.OfficeUserCode);

            _unitOfWork.Repository<MailLog>().Add(ml);
            await _unitOfWork.Repository<MailLog>().Complete();

            return true;

        }

        public async Task<bool> ProfileCreatedMail(AppUser user, string Password, AppUser au = null)
        {
            var mc = await _unitOfWork.Repository<MailConfig>()
                    .GetByNameAsync("MailAction", "PROFILE_CREATED");
            if (mc == null)
            {
                return false;
            }

            string profile = "<tr>" +
                "<td>Name: " + user.DisplayName + "</td></tr><tr>" +
                "<td>Login ID: " + user.UserName + "</td></tr><tr>" +
                "<td>Password: " + Password + "</td></tr>" +
                "<td>Email: " + user.Email + "</td></tr><tr>" +
                "<td>Mobile No: " + user.MobileNo + "</td></tr>";

            var ml = new MailLog();
            var content = mc.MailContent;
            content = content.Replace("{{USER}}", user.DisplayName + " - " + user.OfficeUserCode);

            content = content.Replace("{{TABLE_DATA}}", profile);

            ml.CreatedById = user.OfficeUserId;
            ml.CreatedByName = user.UserName + "-" + user.DisplayName;
            ml.EntityTypeName = user.AppRoleCode;

            ml.CopyFromConfig(mc, content, user.Email);
            ml.MailSubject = ml.MailSubject.Replace("{{OfficeUserCode}}", user.OfficeUserCode);

            if (au != null)
                ml.MailCc = au.Email;

            _unitOfWork.Repository<MailLog>().Add(ml);
            await _unitOfWork.Repository<MailLog>().Complete();

            return true;

        }
        public async Task<bool> ChangePasswordMail(AppUser user, string Password)
        {
            var mc = await _unitOfWork.Repository<MailConfig>()
                    .GetByNameAsync("MailAction", "CHANGE_PASSWORD_SUCCESS");
            if (mc == null)
            {
                return false;
            }

            var ml = new MailLog();
            var content = mc.MailContent;

            ml.CreatedById = user.OfficeUserId;
            ml.CreatedByName = user.UserName + "-" + user.DisplayName;
            ml.CopyFromConfig(mc, content, user.Email);
            ml.MailSubject = ml.MailSubject.Replace("{{OfficeUserCode}}", user.OfficeUserCode);

            _unitOfWork.Repository<MailLog>().Add(ml);
            await _unitOfWork.Repository<MailLog>().Complete();
            return true;

        }


        #endregion


        public async Task<AppRole> ValidateAppRoleAsync(AppRole ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.AppRoleName = ret.AppRoleName.UpperTrim();

            AppRole obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<AppRole>()
                        .GetByNameAsync("AppRoleName", ret.AppRoleName);
                if (dup != null)
                {
                    ret.AddErrorMessage("AppRole already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<AppRole>()
                        .GetEntityWithSpec(new BaseSpecification<AppRole>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown AppRole for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<AppRole>()
                        .GetEntityWithSpec(new BaseSpecification<AppRole>(x => x.AppRoleName == ret.AppRoleName && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("AppRole already exists !!");
                    return ret;
                }
            }
            obj.AppRoleName = ret.AppRoleName;
            obj.IsActive = ret.IsActive;

            return obj;
        }

        public async Task<IImportExcelData<AppRole>> BulkValidateAppRoleAsync(IImportExcelData<AppRole> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            var dupsrc = await _unitOfWork.Repository<AppRole>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].AppRoleName = ret.DataSource[row].AppRoleName.UpperTrim();
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.AppRoleName == ret.DataSource[row].AppRoleName).Any())
                {
                    ret.AddErrorMessage("appRoleName", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.AppRoleName == ret.DataSource[row].AppRoleName).Count() > 1)
                {
                    ret.AddErrorMessage("appRoleName", "Duplicate in List", row);
                }
            }
            return ret;
        }

        public async Task<AppRole> SaveAppRoleAsync(AppRole ret)
        {
            bool isNew = false;
            if (ret.Id == 0)
            {
                isNew = true;
                _unitOfWork.Repository<AppRole>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<AppRole>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<AppRole>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<bool> SaveUploadAppRoleAsync(IReadOnlyList<AppRole> AppRoles, AppUser au)
        {
            foreach (var t in AppRoles)
            {
                var tx = await SaveAppRoleAsync(t);
            }

            return true;
        }

        public async Task<City> ValidateCityAsync(City ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.CityName = ret.CityName.UpperTrim();

            City obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<City>()
                        .GetByNameAsync("CityName", ret.CityName);
                if (dup != null)
                {
                    ret.AddErrorMessage("City already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<City>()
                        .GetEntityWithSpec(new BaseSpecification<City>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown City for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<City>()
                        .GetEntityWithSpec(new BaseSpecification<City>(x => x.CityName == ret.CityName && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("City already exists !!");
                    return ret;
                }
            }
            obj.CityName = ret.CityName;
            obj.IsActive = ret.IsActive;

            return obj;
        }

        public async Task<IImportExcelData<City>> BulkValidateCityAsync(IImportExcelData<City> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            var dupsrc = await _unitOfWork.Repository<City>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].CityName = ret.DataSource[row].CityName.UpperTrim();
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.CityName == ret.DataSource[row].CityName).Any())
                {
                    ret.AddErrorMessage("cityName", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.CityName == ret.DataSource[row].CityName).Count() > 1)
                {
                    ret.AddErrorMessage("cityName", "Duplicate in List", row);
                }
            }
            return ret;
        }

        public async Task<City> SaveCityAsync(City ret)
        {
            var ssts = _config["SyncServerType"];
            bool isNew = false;
            if (ret.Id == 0)
            {
                isNew = true;
                _unitOfWork.Repository<City>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<City>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<City>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<bool> SaveUploadCityAsync(IReadOnlyList<City> Citys, AppUser au)
        {
            foreach (var t in Citys)
            {
                var tx = await SaveCityAsync(t);
            }

            return true;
        }
        public async Task<Country> ValidateCountryAsync(Country ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.CountryName = ret.CountryName.UpperTrim();

            Country obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Country>()
                        .GetByNameAsync("CountryName", ret.CountryName);
                if (dup != null)
                {
                    ret.AddErrorMessage("Country already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<Country>()
                        .GetEntityWithSpec(new BaseSpecification<Country>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown Country for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Country>()
                        .GetEntityWithSpec(new BaseSpecification<Country>(x => x.CountryName == ret.CountryName && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("Country already exists !!");
                    return ret;
                }
            }
            obj.CountryName = ret.CountryName;
            obj.IsActive = ret.IsActive;

            return obj;
        }

        public async Task<IImportExcelData<Country>> BulkValidateCountryAsync(IImportExcelData<Country> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            var dupsrc = await _unitOfWork.Repository<Country>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].CountryName = ret.DataSource[row].CountryName.UpperTrim();
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.CountryName == ret.DataSource[row].CountryName).Any())
                {
                    ret.AddErrorMessage("countryName", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.CountryName == ret.DataSource[row].CountryName).Count() > 1)
                {
                    ret.AddErrorMessage("countryName", "Duplicate in List", row);
                }
            }
            return ret;
        }

        public async Task<Country> SaveCountryAsync(Country ret)
        {
            var ssts = _config["SyncServerType"];
            bool isNew = false;
            if (ret.Id == 0)
            {
                isNew = true;
                _unitOfWork.Repository<Country>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<Country>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<Country>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<bool> SaveUploadCountryAsync(IReadOnlyList<Country> Countrys, AppUser au)
        {
            foreach (var t in Countrys)
            {
                var tx = await SaveCountryAsync(t);
            }

            return true;
        }
        public async Task<Organization> ValidateOrganizationAsync(Organization ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.OrganizationName = ret.OrganizationName.UpperTrim();

            Organization obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Organization>()
                        .GetByNameAsync("OrganizationName", ret.OrganizationName);
                if (dup != null)
                {
                    ret.AddErrorMessage("Organization already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<Organization>()
                        .GetEntityWithSpec(new BaseSpecification<Organization>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown Organization for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Organization>()
                        .GetEntityWithSpec(new BaseSpecification<Organization>(x => x.OrganizationName == ret.OrganizationName && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("Organization already exists !!");
                    return ret;
                }
            }
            obj.OrganizationName = ret.OrganizationName;
            obj.OrganizationCode = ret.OrganizationCode;
            obj.CurrentStartDate = ret.CurrentStartDate;
            obj.CurrentEndDate = ret.CurrentEndDate;
            obj.ApplicationEndDate = ret.ApplicationEndDate;
            obj.ApprovalEndDate = ret.ApprovalEndDate;

            obj.IsActive = ret.IsActive;

            return obj;
        }

        public async Task<IImportExcelData<Organization>> BulkValidateOrganizationAsync(IImportExcelData<Organization> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            var dupsrc = await _unitOfWork.Repository<Organization>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].OrganizationName = ret.DataSource[row].OrganizationName.UpperTrim();
                ret.DataSource[row].OrganizationCode = ret.DataSource[row].OrganizationCode.UpperTrim();
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.OrganizationName == ret.DataSource[row].OrganizationName).Any())
                {
                    ret.AddErrorMessage("organizationName", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.OrganizationName == ret.DataSource[row].OrganizationName).Count() > 1)
                {
                    ret.AddErrorMessage("organizationName", "Duplicate in List", row);
                }
            }
            return ret;
        }

        public async Task<Organization> SaveOrganizationAsync(Organization ret)
        {
            bool isNew = false;
            if (ret.Id == 0)
            {
                isNew = true;
                _unitOfWork.Repository<Organization>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<Organization>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<Organization>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<bool> SaveUploadOrganizationAsync(IReadOnlyList<Organization> Organizations, AppUser au)
        {
            foreach (var t in Organizations)
            {
                var tx = await SaveOrganizationAsync(t);
            }

            return true;
        }
        public async Task<State> ValidateStateAsync(State ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.StateName = ret.StateName.UpperTrim();

            State obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<State>()
                        .GetByNameAsync("StateName", ret.StateName);
                if (dup != null)
                {
                    ret.AddErrorMessage("State already exists !!");
                    return ret;
                }

                var dupx = await _unitOfWork.Repository<State>()
                        .GetEntityWithSpec(new BaseSpecification<State>(x => x.ShortCode == ret.ShortCode));
                if (dupx != null)
                {
                    ret.AddErrorMessage("State Code already exists !!");
                    return ret;
                }
                var dupsx = await _unitOfWork.Repository<State>()
                        .GetEntityWithSpec(new BaseSpecification<State>(x => x.StateGSTCode == ret.StateGSTCode));
                if (dupsx != null)
                {
                    ret.AddErrorMessage("State GST Code already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<State>()
                        .GetEntityWithSpec(new BaseSpecification<State>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown State for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<State>()
                        .GetEntityWithSpec(new BaseSpecification<State>(x => x.StateName == ret.StateName && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("State already exists !!");
                    return ret;
                }
            }
            obj.StateName = ret.StateName;
            obj.IsActive = ret.IsActive;

            return obj;
        }
        public async Task<IImportExcelData<State>> BulkValidateStateAsync(IImportExcelData<State> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            var dupsrc = await _unitOfWork.Repository<State>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].StateName = ret.DataSource[row].StateName.UpperTrim();
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.StateName == ret.DataSource[row].StateName).Any())
                {
                    ret.AddErrorMessage("stateName", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.StateName == ret.DataSource[row].StateName).Count() > 1)
                {
                    ret.AddErrorMessage("stateName", "Duplicate in List", row);
                }
            }
            return ret;
        }
        public async Task<State> SaveStateAsync(State ret)
        {
            var ssts = _config["SyncServerType"];
            bool isNew = false;
            if (ret.Id == 0)
            {
                isNew = true;
                _unitOfWork.Repository<State>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<State>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<State>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<bool> SaveUploadStateAsync(IReadOnlyList<State> States, AppUser au)
        {
            foreach (var t in States)
            {
                var tx = await SaveStateAsync(t);
            }

            return true;
        }
        public async Task<OfficeUser> SaveOfficeUserSecurityAsync(OfficeUser ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            OfficeUser obj;
            if (ret.UCode == null)
            {
                ret.AddErrorMessage("Imcomplete info received !!");
                return ret;
            }
            else
            {
                obj = await _unitOfWork.Repository<OfficeUser>()
                        .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown Office User for editing");
                    return ret;
                }

                //  Check for Existing
                var dup = await _unitOfWork.Repository<OfficeUser>()
                        .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.LoginId == ret.LoginId && x.UCode == ret.UCode));
                if (dup != null)
                {

                }
                var dupx = await _unitOfWork.Repository<OfficeUser>()
                        .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.LoginId == ret.LoginId && x.UCode != ret.UCode));
                if (dupx != null)
                {
                    obj.AddErrorMessage("LoginID already exists !!");

                    return obj;
                }
            }

            if (ret.UCode != null)
            {
                string loginId = obj.LoginId;
                obj.LoginId = ret.LoginId;
                obj.IMEINo = ret.IMEINo;
                obj.AppMode = ret.AppMode;

                _unitOfWork.Repository<OfficeUser>().Update(obj);
                try
                {
                    var res = await _unitOfWork.Repository<OfficeUser>().Complete();
                    if (res <= 0)
                    {
                        ret.AddErrorMessage("Unable to Save");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(obj.AppUserId))
                        {
                            AppUser user = new AppUser
                            {
                                DisplayName = obj.OfficeUserName,
                                MobileNo = obj.MobileNo,
                                Email = obj.WorkEmail,
                                UserName = obj.LoginId,
                                AppRoleCode = obj.AppRoleCode,
                                OfficeUserId = obj.Id,
                                ChangePassword = true,
                                OfficeUserCode = obj.OfficeUserCode
                            };

                            string newpass = "Pass@1234";
                            // do
                            // {
                            //     newpass = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);

                            //     int charchng = -1;

                            //     for(int ix = 0; ix < newpass.Length; ++ix) 
                            //     {
                            //         if (newpass.ToCharArray()[ix] >= 'a' && newpass.ToCharArray()[ix] <= 'z')
                            //         {
                            //             //newpass.ToCharArray()[ix] = newpass.Substring(ix, 1).ToUpper().ToCharArray()[0];
                            //             charchng = ix;
                            //             break;
                            //         }
                            //     }

                            //     if (charchng != -1)
                            //     {
                            //         if (charchng == 0)
                            //         {
                            //             newpass = "#" + newpass.Substring(charchng,1).ToUpper() + newpass.Substring(charchng+1);
                            //         }
                            //         else
                            //         {
                            //             newpass = newpass.Substring(0, charchng-1) + "#" + newpass.Substring(charchng,1).ToUpper() + newpass.Substring(charchng+1);
                            //         }

                            //         var pv = new PasswordValidator<AppUser>();
                            //         var pres = await pv.ValidateAsync(_userManager, user, newpass);
                            //         if (pres.Succeeded)
                            //             break;
                            //     }
                            // } while (true);

                            var cres = await _userManager.CreateAsync(user, newpass);   //
                            if (cres.Succeeded)
                            {
                                obj.AppUserId = user.Id;
                                _unitOfWork.Repository<OfficeUser>().Update(obj);

                                var ress = await _unitOfWork.Complete();
                                if (ress <= 0)
                                {
                                    ret.AddErrorMessage("Unable to Save");
                                }
                            }
                        }
                        else
                        {
                            var ua = await _userManager.FindByIdAsync(obj.AppUserId);

                            if (ua != null)
                            {
                                ua.Email = obj.WorkEmail;
                                ua.DisplayName = obj.OfficeUserName;
                                ua.AppRoleCode = obj.AppRoleCode;
                                ua.UserName = ret.LoginId;
                                ua.MobileNo = obj.MobileNo;

                                ua.OfficeUserId = obj.Id;
                                ua.OfficeUserCode = obj.OfficeUserCode;

                                var cres = await _userManager.UpdateAsync(ua);
                                if (cres.Succeeded)
                                {
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ret.AddErrorMessage("Exception: " + ex.Message);
                    await AddActionLogAsync("SaveEmployeeSecurityAsync", ex.Message, "LoginID", obj.LoginId);
                }
            }

            return obj;
        }
        public async Task<string> GetOfficeUserPassword(AppUser ui)
        {
            string newpass = null;
            do
            {
                newpass = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);

                int charchng = -1;

                for (int ix = 0; ix < newpass.Length; ++ix)
                {
                    if (newpass.ToCharArray()[ix] >= 'a' && newpass.ToCharArray()[ix] <= 'z')
                    {
                        //newpass.ToCharArray()[ix] = newpass.Substring(ix, 1).ToUpper().ToCharArray()[0];
                        charchng = ix;
                        break;
                    }
                }

                if (charchng != -1)
                {
                    if (charchng == 0)
                    {
                        newpass = "#" + newpass.Substring(charchng, 1).ToUpper() + newpass.Substring(charchng + 1);
                    }
                    else
                    {
                        newpass = newpass.Substring(0, charchng - 1) + "#" + newpass.Substring(charchng, 1).ToUpper() + newpass.Substring(charchng + 1);
                    }

                    var pv = new PasswordValidator<AppUser>();
                    var pres = await pv.ValidateAsync(_userManager, ui, newpass);
                    if (pres.Succeeded)
                        break;
                }
            } while (true);
            newpass = "Pass@1234";
            return newpass;
        }
        public async Task<string> SetResetFailedCount(string username, OfficeUser? ou)
        {
            OfficeUser ou1 = null;

            if (username == null && ou != null)
            {
                username = ou.LoginId;
            }
            else
            {
                username = username.ToUpper();
            }

            try
            {
                var ofc = await _unitOfWork.Repository<OfficeUser>()
                    .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.LoginId == username));

                if (ofc != null)
                {
                    ou1 = ofc;
                    ou1.FailedLoginCount = 0;
                    _unitOfWork.Repository<OfficeUser>().Update(ou1);

                    await _unitOfWork.Complete();
                }
                else
                {
                    return null;
                }

                return "Reset";
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<MailConfig> ValidateMailConfigAsync(MailConfig ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.MailAction = ret.MailAction.UpperTrim();
            ret.MailFromName = ret.MailFromName.UpperTrim();

            MailConfig obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<MailConfig>()
                        .GetByNameAsync("MailAction", ret.MailAction);
                if (dup != null)
                {
                    ret.AddErrorMessage("Mail Action already exists !!");
                    return ret;
                }
                // var dupx = await _unitOfWork.Repository<MailConfig>()
                //         .GetEntityWithSpec(new BaseSpecification<MailConfig>(x => x.ZoneCode == ret.ZoneCode));
                // if (dupx != null)
                // {
                //     ret.AddErrorMessage("Zone Code already exists !!");
                //     return ret;
                // }
            }
            else
            {
                obj = await _unitOfWork.Repository<MailConfig>()
                        .GetEntityWithSpec(new BaseSpecification<MailConfig>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown Zone for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<MailConfig>()
                        .GetEntityWithSpec(new BaseSpecification<MailConfig>(x => x.MailAction == ret.MailAction && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("Mail Action already exists !!");
                    return ret;
                }

            }
            obj.MailAction = ret.MailAction;
            obj.MailDescription = ret.MailDescription;
            obj.MailFromName = ret.MailFromName;
            obj.MailFromEmail = ret.MailFromEmail;
            obj.MailBcc = ret.MailBcc;
            obj.MailCc = ret.MailCc;
            obj.MailSubject = ret.MailSubject;
            obj.MailContent = ret.MailContent;
            obj.SmtpServer = ret.SmtpServer;
            obj.SmtpPort = ret.SmtpPort;
            obj.IsAuthenticationReq = ret.IsAuthenticationReq;
            obj.IsSslReq = ret.IsSslReq;
            obj.IsHtml = ret.IsHtml;
            obj.IsActive = true;

            return obj;
        }
        public async Task<MailConfig> SaveMailConfigAsync(MailConfig ret)
        {
            if (ret.Id == 0)
            {
                _unitOfWork.Repository<MailConfig>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<MailConfig>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<MailConfig>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<IReadOnlyList<MailLog>> GetMailLogsAsync(DateTime DtFrom, DateTime DtTo)
        {
            var sdt = (await _unitOfWork.Repository<MailLog>()
              .ListAsync(new BaseSpecification<MailLog>(x => x.MailQueuedOn >= DtFrom && x.MailQueuedOn <= DtTo)));

            if (sdt == null)
            {
                return null;
            }
            return sdt;

        }
        public async Task<IReadOnlyList<ActionLog>> GetActionLog(DateTime DtFrom, DateTime DtTo)
        {
            // if (au == null)
            // {
            //     ret.AddErrorMessage("Unknown User");
            //     return ret;
            // }
            var elb = await _unitOfWork.Repository<ActionLog>()
                .ListAsync(new BaseSpecification<ActionLog>(x => x.CreatedOn >= DtFrom && x.CreatedOn <= DtTo));

            if (elb != null && elb.Count > 0)
            {
                elb = elb.OrderByDescending(x => x.CreatedOn).ToList();
            }

            return elb;
        }

        public async Task<AppRole> ValidateAppRoleDeleteAsync(Guid ucode, string reason, AppUser au)
        {
            if (au == null)
            {
                return null;
            }

            AppRole obj = await _unitOfWork.Repository<AppRole>()
                    .GetEntityWithSpec(new BaseSpecification<AppRole>(x => x.UCode == ucode && x.IsDeleted == false));
            if (obj != null)
            {
                var rr = "";
                if (!String.IsNullOrEmpty(obj.LogHistory))
                {
                    rr = reason;
                }

                //  Store History in Descending order
                rr = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow)
                + " || " + au.DisplayName
                + " || " + reason
                + "\r\n"
                + rr;

                obj.IsDeleted = true;
                obj.LogHistory = rr;
                return obj;
            }
            else
            {
                obj.AddErrorMessage("Incomplete info received");
                return obj;
            }

            return obj;
        }
        public async Task<AppRole> DeleteAppRoleAsync(AppRole ret, AppUser au)
        {
            var ssts = _config["SyncServerType"];
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }


            if (ret.UCode != null)
            {

                _unitOfWork.Repository<AppRole>().Update(ret);

                // Action Log
                ActionLog actlg = new ActionLog();
                actlg.ModuleName = "App Role";
                actlg.Description = "Deletion of App Role";
                actlg.EntityName = "App Role Code";
                actlg.EntityValue = ret.AppRoleCode;
                actlg.ActionName = "Delete";
                actlg.ClientIP = "";
                actlg.ClientBrowser = "";
                if (ssts == "External")
                {
                    actlg.CreatedById = 0;
                    actlg.CreatedByName = "";
                }
                else
                {
                    actlg.CreatedById = au.OfficeUserId;
                    actlg.CreatedByName = au.UserName + "-" + au.DisplayName;

                }
                actlg.CreatedOn = DateTime.UtcNow;
                _unitOfWork.Repository<ActionLog>().Add(actlg);
            }
            try
            {
                var res = await _unitOfWork.Repository<ActionLog>().Complete();

                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<Core.Entities.Department> ValidateDepartmentDeleteAsync(Guid ucode, string reason, AppUser au)
        {
            if (au == null)
            {
                return null;
            }

            Core.Entities.Department obj = await _unitOfWork.Repository<Core.Entities.Department>()
                    .GetEntityWithSpec(new BaseSpecification<Core.Entities.Department>(x => x.UCode == ucode && x.IsDeleted == false));
            if (obj != null)
            {
                var rr = "";
                if (!String.IsNullOrEmpty(obj.LogHistory))
                {
                    rr = reason;
                }

                //  Store History in Descending order
                rr = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow)
                + " || " + au.DisplayName
                + " || " + reason
                + "\r\n"
                + rr;

                obj.IsDeleted = true;
                obj.LogHistory = rr;
                return obj;
            }
            else
            {
                obj.AddErrorMessage("Incomplete info received");
                return obj;
            }

            return obj;
        }
        public async Task<Core.Entities.Department> DeleteDepartmentAsync(Core.Entities.Department ret, AppUser au)
        {
            var ssts = _config["SyncServerType"];
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            if (ret.UCode != null)
            {

                _unitOfWork.Repository<Core.Entities.Department>().Update(ret);

                // Action Log
                ActionLog actlg = new ActionLog();
                actlg.ModuleName = "Department";
                actlg.Description = "Deletion of Department";
                actlg.EntityName = "DepartmentCode";
                actlg.EntityValue = ret.DepartmentCode;
                actlg.ActionName = "Delete";
                actlg.ClientIP = "";
                actlg.ClientBrowser = "";
                if (ssts == "External")
                {
                    actlg.CreatedById = 0;
                    actlg.CreatedByName = "";
                }
                else
                {
                    actlg.CreatedById = au.OfficeUserId;
                    actlg.CreatedByName = au.UserName + "-" + au.DisplayName;
                }
                actlg.CreatedOn = DateTime.UtcNow;
                _unitOfWork.Repository<ActionLog>().Add(actlg);
            }
            try
            {
                var res = await _unitOfWork.Repository<ActionLog>().Complete();

                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<Organization> ValidateOrganizationDeleteAsync(Guid ucode, string reason, AppUser au)
        {
            if (au == null)
            {
                return null;
            }

            Organization obj = await _unitOfWork.Repository<Organization>()
                    .GetEntityWithSpec(new BaseSpecification<Organization>(x => x.UCode == ucode && x.IsDeleted == false));
            if (obj != null)
            {
                var rr = "";
                if (!String.IsNullOrEmpty(obj.LogHistory))
                {
                    rr = reason;
                }

                //  Store History in Descending order
                rr = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow)
                + " || " + au.DisplayName
                + " || " + reason
                + "\r\n"
                + rr;

                obj.IsDeleted = true;
                obj.LogHistory = rr;
                return obj;
            }
            else
            {
                obj.AddErrorMessage("Incomplete info received");
                return obj;
            }

            return obj;
        }
        public async Task<Organization> DeleteOrganizationAsync(Organization ret, AppUser au)
        {
            var ssts = _config["SyncServerType"];
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            if (ret.UCode != null)
            {

                _unitOfWork.Repository<Organization>().Update(ret);

                // Action Log
                ActionLog actlg = new ActionLog();
                actlg.ModuleName = "Organizaion";
                actlg.Description = "Deletion of Organization";
                actlg.EntityName = "Organization Name";
                actlg.EntityValue = ret.OrganizationName;
                actlg.ActionName = "Delete";
                actlg.ClientIP = "";
                actlg.ClientBrowser = "";
                if (ssts == "External")
                {
                    actlg.CreatedById = 0;
                    actlg.CreatedByName = "";
                }
                else
                {
                    actlg.CreatedById = au.OfficeUserId;
                    actlg.CreatedByName = au.UserName + "-" + au.DisplayName;

                }
                actlg.CreatedOn = DateTime.UtcNow;
                _unitOfWork.Repository<ActionLog>().Add(actlg);
            }
            try
            {
                var res = await _unitOfWork.Repository<ActionLog>().Complete();

                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<Country> ValidateCountryDeleteAsync(Guid ucode, string reason, AppUser au)
        {
            if (au == null)
            {
                return null;
            }

            Country obj = await _unitOfWork.Repository<Country>()
                    .GetEntityWithSpec(new BaseSpecification<Country>(x => x.UCode == ucode && x.IsDeleted == false));
            if (obj != null)
            {
                var rr = "";
                if (!String.IsNullOrEmpty(obj.LogHistory))
                {
                    rr = reason;
                }

                //  Store History in Descending order
                rr = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow)
                + " || " + au.DisplayName
                + " || " + reason
                + "\r\n"
                + rr;

                obj.IsDeleted = true;
                obj.LogHistory = rr;
                return obj;
            }
            else
            {
                obj.AddErrorMessage("Incomplete info received");
                return obj;
            }

            return obj;
        }
        public async Task<Country> DeleteCountryAsync(Country ret, AppUser au)
        {
            var ssts = _config["SyncServerType"];
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            if (ret.UCode != null)
            {

                _unitOfWork.Repository<Country>().Update(ret);

                // Action Log
                ActionLog actlg = new ActionLog();
                actlg.ModuleName = "Country";
                actlg.Description = "Deletion of Country";
                actlg.EntityName = "Country Name";
                actlg.EntityValue = ret.CountryName;
                actlg.ActionName = "Delete";
                actlg.ClientIP = "";
                actlg.ClientBrowser = "";
                if (ssts == "External")
                {
                    actlg.CreatedById = 0;
                    actlg.CreatedByName = "";
                }
                else
                {
                    actlg.CreatedById = au.OfficeUserId;
                    actlg.CreatedByName = au.UserName + "-" + au.DisplayName;

                }
                actlg.CreatedOn = DateTime.UtcNow;
                _unitOfWork.Repository<ActionLog>().Add(actlg);
            }
            try
            {
                var res = await _unitOfWork.Repository<ActionLog>().Complete();

                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<State> ValidateStateDeleteAsync(Guid ucode, string reason, AppUser au)
        {
            if (au == null)
            {
                return null;
            }

            State obj = await _unitOfWork.Repository<State>()
                    .GetEntityWithSpec(new BaseSpecification<State>(x => x.UCode == ucode && x.IsDeleted == false));
            if (obj != null)
            {
                var rr = "";
                if (!String.IsNullOrEmpty(obj.LogHistory))
                {
                    rr = reason;
                }

                //  Store History in Descending order
                rr = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow)
                + " || " + au.DisplayName
                + " || " + reason
                + "\r\n"
                + rr;

                obj.IsDeleted = true;
                obj.LogHistory = rr;
                return obj;
            }
            else
            {
                obj.AddErrorMessage("Incomplete info received");
                return obj;
            }

            return obj;
        }
        public async Task<State> DeleteStateAsync(State ret, AppUser au)
        {
            var ssts = _config["SyncServerType"];
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            if (ret.UCode != null)
            {

                _unitOfWork.Repository<State>().Update(ret);

                // Action Log
                ActionLog actlg = new ActionLog();
                actlg.ModuleName = "State";
                actlg.Description = "Deletion of State";
                actlg.EntityName = "State Short Code";
                actlg.EntityValue = ret.ShortCode;
                actlg.ActionName = "Delete";
                actlg.ClientIP = "";
                actlg.ClientBrowser = "";
                if (ssts == "External")
                {
                    actlg.CreatedById = 0;
                    actlg.CreatedByName = "";
                }
                else
                {
                    actlg.CreatedById = au.OfficeUserId;
                    actlg.CreatedByName = au.UserName + "-" + au.DisplayName;

                }
                actlg.CreatedOn = DateTime.UtcNow;
                _unitOfWork.Repository<ActionLog>().Add(actlg);
            }
            try
            {
                var res = await _unitOfWork.Repository<ActionLog>().Complete();

                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<City> ValidateCityDeleteAsync(Guid ucode, string reason, AppUser au)
        {
            if (au == null)
            {
                return null;
            }

            City obj = await _unitOfWork.Repository<City>()
                    .GetEntityWithSpec(new BaseSpecification<City>(x => x.UCode == ucode && x.IsDeleted == false));
            if (obj != null)
            {
                var rr = "";
                if (!String.IsNullOrEmpty(obj.LogHistory))
                {
                    rr = reason;
                }

                //  Store History in Descending order
                rr = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow)
                + " || " + au.DisplayName
                + " || " + reason
                + "\r\n"
                + rr;

                obj.IsDeleted = true;
                obj.LogHistory = rr;
                return obj;
            }
            else
            {
                obj.AddErrorMessage("Incomplete info received");
                return obj;
            }

            return obj;
        }
        public async Task<City> DeleteCityAsync(City ret, AppUser au)
        {
            var ssts = _config["SyncServerType"];
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User"); return ret;
            }

            if (ret.UCode != null)
            {

                _unitOfWork.Repository<City>().Update(ret);

                // Action Log
                ActionLog actlg = new ActionLog();
                actlg.ModuleName = "City";
                actlg.Description = "Deletion of City";
                actlg.EntityName = "City Name";
                actlg.EntityValue = ret.CityName;
                actlg.ActionName = "Delete";
                actlg.ClientIP = "";
                actlg.ClientBrowser = "";
                if (ssts == "External")
                {
                    actlg.CreatedById = 0;
                    actlg.CreatedByName = "";
                }
                else
                {
                    actlg.CreatedById = au.OfficeUserId;
                    actlg.CreatedByName = au.UserName + "-" + au.DisplayName;

                }
                actlg.CreatedOn = DateTime.UtcNow;
                _unitOfWork.Repository<ActionLog>().Add(actlg);
            }
            try
            {
                var res = await _unitOfWork.Repository<ActionLog>().Complete();

                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<SysData> SaveSysAsync(SysData ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }
            SysData obj;

            obj = await _unitOfWork.Repository<SysData>()
                    .GetEntityWithSpec(new BaseSpecification<SysData>(x => x.FieldName == ret.FieldName));
            if (obj == null)
            {
                obj = new SysData();
                obj.IsActive = true;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();
                // ret.AddErrorMessage("Unknown password policy for editing");
                // return ret;
            }

            obj.FieldName = ret.FieldName;
            obj.FieldValue = ret.FieldValue;

            try
            {
                if (obj.Id == 0)
                    _unitOfWork.Repository<SysData>().Add(obj);
                else
                    _unitOfWork.Repository<SysData>().Update(obj);

                var res = await _unitOfWork.Repository<SysData>().Complete();

                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }

            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }
            return obj;
        }

        public async Task<ActionLog> AddActionLogAsync(ActionLog actlg)
        {
            // if (au == null)
            // {
            //     ret.AddErrorMessage("Unknown User");
            //     return ret;
            // }
            if (actlg.ClientIP == null)
                actlg.ClientIP = "";
            actlg.ClientBrowser = "";
            actlg.CreatedById = 0;
            actlg.CreatedByName = "";
            actlg.CreatedOn = DateTime.UtcNow;
            // actlg.ClientType = "Web";
            _unitOfWork.Repository<ActionLog>().Add(actlg);
            try
            {
                var res = await _unitOfWork.Repository<ActionLog>().Complete();

                // if (res <= 0)
                // {
                //     ret.AddErrorMessage("Unable to Save");
                // }

            }
            catch (Exception ex)
            {
                // ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return actlg;
        }

        public async Task<int> AddActionLogAsync(string ActionName, string Description, string EntityName = null, string EntityValue = null)
        {
            // if (au == null)
            // {
            //     ret.AddErrorMessage("Unknown User");
            //     return ret;
            // }

            int res = -99;
            try
            {
                ActionLog actlg = new ActionLog();

                actlg.ActionName = ActionName;
                actlg.Description = Description;
                actlg.EntityName = EntityName;
                actlg.EntityValue = EntityValue;
                if (actlg.ClientIP == null)
                    actlg.ClientIP = "";
                actlg.ClientBrowser = "";
                actlg.CreatedById = 0;
                actlg.CreatedByName = "";
                actlg.CreatedOn = DateTime.UtcNow;

                _unitOfWork.Repository<ActionLog>().Add(actlg);
                res = await _unitOfWork.Repository<ActionLog>().Complete();

                // if (res <= 0)
                // {
                //     ret.AddErrorMessage("Unable to Save");
                // }
            }
            catch (Exception ex)
            {
                // ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return res;
        }
        public async Task<IReadOnlyList<OfficeUser>> GetOfficeUserForAppRoleCode(string AppRoleCode, string OrganizationName)
        {
            var emp = await _unitOfWork.Repository<OfficeUser>()
                .ListAsync(new BaseSpecification<OfficeUser>(x => x.AppRoleCode == AppRoleCode && (x.OrganizationName == null || x.OrganizationName == OrganizationName)
                && x.AppUserId != null));

            if (emp != null)
                emp = emp.OrderBy(x => x.OfficeUserName).ToList();

            return emp;
        }
        public async Task<IReadOnlyList<UserNavMenu>> GetNavMenuOfUserManageAsync(string OfficeUserCode, string AppRoleCode)
        {
            var emp = await _unitOfWork.Repository<OfficeUser>()
                        .GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.OfficeUserCode == OfficeUserCode &&
                        x.AppRoleCode == AppRoleCode && x.IsDeleted == false));
            if (emp != null)
            {
                var nms = await _unitOfWork.Repository<UserNavMenu>()
                 .ListAsync(new BaseSpecification<UserNavMenu>(x => x.AppUserId == emp.AppUserId));

                return nms;
            }
            else
            {
                return null;
            }
        }

        public async Task<IReadOnlyList<Designation>> GetDesignationsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<Designation>().ListAllAsync();

            return r;
        }

        public async Task<Designation> GetDesignationByNameAsync(string DesignationName)
        {
            var r = await _unitOfWork.Repository<Designation>()
                    .GetByNameAsync("DesignationName", DesignationName);

            return r;
        }

        public async Task<Designation> GetDesignationByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<Designation>()
                    .GetByNameAsync("UCode", Ucode);

            return r;
        }

        public async Task<Designation> ValidateDesignationAsync(Designation ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.DesignationName = ret.DesignationName.UpperTrim();

            Designation obj;
            if (ret.UCode == null || ret.UCode.ToString().Replace("-", "").StartsWith("0000000000"))
            {
                obj = ret;
                obj.Id = 0;
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.UCode = Guid.NewGuid();

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Designation>()
                        .GetByNameAsync("DesignationName", ret.DesignationName);
                if (dup != null)
                {
                    ret.AddErrorMessage("Designation already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<Designation>()
                        .GetEntityWithSpec(new BaseSpecification<Designation>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown Designation for editing");
                    return ret;
                }

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<Designation>()
                        .GetEntityWithSpec(new BaseSpecification<Designation>(x => x.DesignationName == ret.DesignationName && x.UCode != ret.UCode));
                if (dup != null)
                {
                    ret.AddErrorMessage("Designation already exists !!");
                    return ret;
                }
            }
            obj.DesignationName = ret.DesignationName;
            obj.IsActive = ret.IsActive;
            obj.OrganizationNameInclude = ret.OrganizationNameInclude;

            return obj;
        }

        public async Task<IImportExcelData<Designation>> BulkValidateDesignationAsync(IImportExcelData<Designation> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            var dupsrc = await _unitOfWork.Repository<Designation>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].DesignationName = ret.DataSource[row].DesignationName.UpperTrim();
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.DesignationName == ret.DataSource[row].DesignationName).Any())
                {
                    ret.AddErrorMessage("designationName", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.DesignationName == ret.DataSource[row].DesignationName).Count() > 1)
                {
                    ret.AddErrorMessage("designationName", "Duplicate in List", row);
                }
            }
            return ret;
        }

        public async Task<Designation> SaveDesignationAsync(Designation ret)
        {
            if (ret.Id == 0)
            {
                _unitOfWork.Repository<Designation>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<Designation>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<Designation>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<bool> SaveUploadDesignationAsync(IReadOnlyList<Designation> Designations, AppUser au)
        {
            foreach (var t in Designations)
            {
                var tx = await SaveDesignationAsync(t);
            }

            return true;
        }

        public async Task<IReadOnlyList<OfficeUser>> GetOfficeUsers()
        {
            var o = (await _unitOfWork.Repository<OfficeUser>().ListAsync(new OfficeUsersSpecification()));

            return o;
        }

        public async Task<IReadOnlyList<OfficeUser>> GetOfficeUsers(ISpecification<OfficeUser> spec)
        {
            var o = (await _unitOfWork.Repository<OfficeUser>().ListAsync(spec));

            return o;
        }

        public async Task<OfficeUser> GetOfficeUserById(int Id)
        {
            var o = (await _unitOfWork.Repository<OfficeUser>().ListAsync(new OfficeUsersSpecification(Id)))
                    .FirstOrDefault();

            return o;
        }
        public async Task<OfficeUser> GetOfficeUserByEmail(string WorkEmail)
        {
            var o = (await _unitOfWork.Repository<OfficeUser>().ListAsync(new OfficeUsersSpecification(WorkEmail)))
                    .FirstOrDefault();

            return o;
        }

        public async Task<OfficeUser> SaveOfficeUser(OfficeUser officeUser, OfficeUser CurrentUser)
        {
            bool exec = false;

            var d = await _unitOfWork.Repository<Core.Entities.Department>().GetEntityWithSpec(new BaseSpecification<Core.Entities.Department>(x => x.DepartmentName == officeUser.Department.DepartmentName));
            if (d == null)
            {
                d = new Core.Entities.Department();
                d.DepartmentName = officeUser.Department.DepartmentName;
                d.CreatedById = 1;

                exec = true;

                _unitOfWork.Repository<Core.Entities.Department>().Add(d);
            }
            var ar = await _unitOfWork.Repository<AppRole>().GetEntityWithSpec(new BaseSpecification<AppRole>(x => x.AppRoleName == officeUser.AppRole.AppRoleName));
            if (ar == null)
            {
                ar = new AppRole();
                ar.AppRoleName = officeUser.AppRole.AppRoleName;
                ar.CreatedById = 1;

                exec = true;

                _unitOfWork.Repository<AppRole>().Add(ar);
            }

            if (exec)
            {
                var rx = await _unitOfWork.Complete();

                if (rx <= 0)
                {
                    return null;
                }
                exec = false;
            }

            var ouspec = new OfficeUsersSpecification(0, officeUser.AppUserId);

            OfficeUser existOffUser = await _unitOfWork.Repository<OfficeUser>()
                    .GetEntityWithSpec(ouspec);
            if (existOffUser == null)
            {
                var existmail = await _unitOfWork.Repository<OfficeUser>().GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.WorkEmail == officeUser.WorkEmail));

                if (existmail == null)
                {
                    var ui = await _userManager.FindByEmailAsync(officeUser.WorkEmail);

                    if (ui == null)
                    {
                        var u = new AppUser()
                        {
                            AppRoleCode = officeUser.AppRole.AppRoleCode,
                            DisplayName = officeUser.OfficeUserName,
                            OfficeUserCode = officeUser.OfficeUserCode,
                        };
                        u.Email = officeUser.WorkEmail;
                        u.UserName = officeUser.WorkEmail;
                        u.ChangePassword = true;
                        await _userManager.CreateAsync(u, "Pass@1234");
                        officeUser.AppUserId = u.Id;
                    }
                    else
                    {
                        officeUser.AppUserId = ui.Id;
                    }

                    officeUser.DepartmentId = d.Id;
                    officeUser.AppRoleId = ar.Id;
                    officeUser.Department = d;
                    officeUser.AppRole = ar;

                    _unitOfWork.Repository<OfficeUser>().Add(officeUser);
                }
                else
                {
                }
            }
            else
            {
                var existmails = await _unitOfWork.Repository<OfficeUser>().GetEntityWithSpec(new BaseSpecification<OfficeUser>(x => x.WorkEmail == officeUser.WorkEmail));

                if (existmails != null)
                {
                    if (existmails.AppUserId == officeUser.AppUserId)
                    {
                        var ui = await _userManager.FindByEmailAsync(existOffUser.WorkEmail);

                        if (ui == null)
                        {
                        }
                        else
                        {
                            ui.Email = officeUser.WorkEmail;
                            ui.DisplayName = officeUser.OfficeUserName;
                            ui.UserName = officeUser.WorkEmail;

                            await _userManager.UpdateAsync(ui);
                            officeUser.AppUserId = ui.Id;
                        }

                        existOffUser.DepartmentId = d.Id;
                        existOffUser.AppRoleId = ar.Id;
                        existOffUser.OfficeUserName = officeUser.OfficeUserName;
                        existOffUser.Department = d;
                        existOffUser.AppRole = ar;
                        existOffUser.WorkEmail = officeUser.WorkEmail;

                        _unitOfWork.Repository<OfficeUser>().Update(existOffUser);
                    }
                }
                else
                {
                    var ui = await _userManager.FindByEmailAsync(existOffUser.WorkEmail);

                    if (ui == null)
                    {
                    }
                    else
                    {
                        ui.Email = officeUser.WorkEmail;
                        ui.DisplayName = officeUser.OfficeUserName;
                        ui.UserName = officeUser.WorkEmail;

                        await _userManager.UpdateAsync(ui);
                        officeUser.AppUserId = ui.Id;
                    }
                    existOffUser.DepartmentId = d.Id;
                    existOffUser.AppRoleId = ar.Id;
                    existOffUser.OfficeUserName = officeUser.OfficeUserName;
                    existOffUser.Department = d;
                    existOffUser.AppRole = ar;
                    existOffUser.WorkEmail = officeUser.WorkEmail;

                    _unitOfWork.Repository<OfficeUser>().Update(existOffUser);

                }
            }

            //  TODO: save to db
            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                return null;
            }

            var navmenulist = await _unitOfWork.Repository<NavMenu>()
                    .ListAllAsync();

            var o = officeUser;

            if (existOffUser != null)
                o = existOffUser;

            if (existOffUser != null)
            {
                return existOffUser;
            }

            return officeUser;
        }

        public async Task<bool> SaveUploadOfficeUser(IReadOnlyList<OfficeUser> officeUsers, OfficeUser CurrentUser)
        {
            var ds = await _unitOfWork.Repository<Core.Entities.Department>().ListAllAsync();
            if (ds == null)
            {

            }
            var ars = await _unitOfWork.Repository<AppRole>().ListAllAsync();
            if (ars == null)
            {

            }
            var ous = await _unitOfWork.Repository<OfficeUser>().ListAllAsync();        //  Check for existing user
            if (ous == null)
            {

            }

            foreach (var officeUser in officeUsers)
            {
                var d = new Core.Entities.Department();
                if (!String.IsNullOrEmpty(officeUser.Department.DepartmentCode))
                {
                    d = ds.Where(x => x.DepartmentCode == officeUser.Department.DepartmentCode).FirstOrDefault();
                }
                else
                {
                    d = ds.Where(x => x.DepartmentName == officeUser.Department.DepartmentName).FirstOrDefault();
                }

                var ar = ars.Where(x => x.AppRoleName.ToUpper() == officeUser.AppRole.AppRoleName.ToUpper()).FirstOrDefault();

                var ui = await _userManager.FindByEmailAsync(officeUser.WorkEmail);
                if (ui == null)
                {
                    var u = new AppUser()
                    {
                        AppRoleCode = officeUser.AppRole.AppRoleCode,
                        DisplayName = officeUser.OfficeUserName,
                        OfficeUserCode = officeUser.OfficeUserCode,
                    };

                    u.Email = officeUser.WorkEmail;
                    u.UserName = officeUser.WorkEmail;
                    u.ChangePassword = true;
                    await _userManager.CreateAsync(u, "Pass@1234");
                    officeUser.AppUserId = u.Id;
                }
                else
                {
                    officeUser.AppUserId = ui.Id;
                }

                if (!ous.Where(x => x.WorkEmail == officeUser.WorkEmail).Any())
                {
                    var newuser = new OfficeUser()
                    {
                        OfficeUserName = officeUser.OfficeUserName,
                        WorkEmail = officeUser.WorkEmail,
                        AppRoleCode = ar.AppRoleCode,
                        AppUserId = officeUser.AppUserId,
                        AppRole = ar,
                        OrganizationName = officeUser.OrganizationName,
                    };

                    if (d != null && ar != null)
                    {
                        newuser.CreatedOn = System.DateTime.UtcNow;
                        newuser.CreatedById = CurrentUser.Id;
                        newuser.CreatedByName = CurrentUser.OfficeUserName;
                        newuser.IsActive = true;
                        newuser.IsDeleted = false;
                        newuser.IsAdmin = false;
                        newuser.DepartmentId = d.Id;
                        newuser.AppRoleId = ar.Id;

                        newuser.Department = d;

                        _unitOfWork.Repository<OfficeUser>().Add(newuser);

                        //  TODO: save to db
                        var result = await _unitOfWork.Complete();

                        if (result > 0)
                        {
                        }
                    }
                }
                else
                {
                    var newuser = ous.Where(x => x.WorkEmail == officeUser.WorkEmail).FirstOrDefault();

                    if (d != null && ar != null && newuser != null)
                    {
                        newuser.OfficeUserName = officeUser.OfficeUserName;
                        newuser.WorkEmail = officeUser.WorkEmail;
                        newuser.AppUserId = officeUser.AppUserId;
                        newuser.DepartmentId = d.Id;
                        newuser.AppRoleId = ar.Id;

                        newuser.Department = d;
                        newuser.AppRole = ar;

                        _unitOfWork.Repository<OfficeUser>().Update(newuser);

                        //  TODO: save to db
                        var result = await _unitOfWork.Complete();

                        if (result > 0)
                        {
                        }
                    }
                }
            }

            return true;
        }
        public async Task<AppUser> ChangePassword(AppUser currentUser, string NewPassword)
        {
            bool exec = false;

            string newpass = NewPassword;

            var ui = currentUser;

            if (ui == null)
            {
                return null;
            }

            var pv = new PasswordValidator<AppUser>();
            var pres = await pv.ValidateAsync(_userManager, ui, newpass);
            if (!pres.Succeeded)
                return null;

            string token = "";
            try
            {
                token = await _userManager.GeneratePasswordResetTokenAsync(ui);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            var rres = await _userManager.ResetPasswordAsync(ui, token, NewPassword);
            if (rres.Succeeded)
            {
                return ui;
            }

            return null;
        }
        public async Task<IReadOnlyList<patient>> GetPatientsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<patient>().ListAllAsync();
            r = r.OrderBy(x => x.full_name).ToList();

            return r;
        }
        public async Task<patient> GetPatientByRegNoAsync(string RegNo)
        {
            var r = await _unitOfWork.Repository<patient>()
                    .GetByNameAsync("RegNo", RegNo);

            return r;
        }
        public async Task<patient> GetPatientByPatientId(int patient_id)
        {
            var r = await _unitOfWork.Repository<patient>().GetByIntPropertyAsync("Id", patient_id);
            return r;
        }
        public async Task<AppUser> GetdoctorBydoctorId(int doctor_id)
        {
            var r = await _userManager.Users
        .FirstOrDefaultAsync(u => u.OfficeUserId == doctor_id);
            return r;
        }
        public async Task<patient> GetPatientByCodeAsync(string Ucode)
        {
            var r = await _unitOfWork.Repository<patient>()
                   .GetByNameAsync("UCode", Ucode);

            return r;
        }
        public async Task<IReadOnlyList<patient>> GetPatientByhistoryAsync(AppUser appUser)
        {
            var historyPatients = await _unitOfWork.Repository<patient>()
                       .GetEntityListWithSpec(new BaseSpecification<patient>(x => x.history == false && x.TypeofPatient != "New")
                       );
            return historyPatients;
        }
        public async Task<patient> ValidatePatientAsync(patient ret, AppUser au)
        {
            if (au == null)
            {
                ret.AddErrorMessage("Unknown User");
                return ret;
            }

            ret.full_name = ret.full_name.UpperTrim();

            patient obj;
          
            if (ret.UCode == Guid.Empty)
            {
                obj = ret;
                obj.Id = 0;
                obj.UCode = Guid.NewGuid();
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;               

                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<patient>()
                        .GetEntityWithSpec(new BaseSpecification<patient>(x => x.mobileNo == ret.mobileNo)
                        );
                if (dup != null)
                {
                    ret.AddErrorMessage("Patient already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<patient>()
                        .GetEntityWithSpec(new BaseSpecification<patient>(x => x.UCode == ret.UCode));
                if (obj == null)
                {
                    ret.AddErrorMessage("Unknown Patient for editing");
                    return ret;
                }
               
            }
            obj.full_name = ret.full_name;
            obj.address = ret.address;
            obj.emailId = ret.emailId;
            obj.ADoctorId = ret.ADoctorId;
            obj.city = ret.city;
            obj.mobileNo = ret.mobileNo;
            obj.IsActive = ret.IsActive;

            return obj;
        }
        public async Task<patient> SavePatientAsync(patient ret)
        {
            var ssts = _config["SyncServerType"];
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            if (ret.DOB.HasValue)
            {
                ret.DOB = ret.DOB.Value.ToUniversalTime();
            }
            if (ret.Id == 0)
            {                
                isNew = true;              
                ret.RegNo = await GenerateNewRegNo();
                _unitOfWork.Repository<patient>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<patient>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<patient>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<patient>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<complaints> SaveComplaintsAsync(complaints ret, AppUser cu)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById=cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;

            if (ret.Id == 0)
            {
                isNew = true;
                ret.ExtraValue1 = "Saved";
                _unitOfWork.Repository<complaints>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<complaints>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<complaints>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<complaints>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }
            return ret;
        }
        public async Task<physicalexam> SavePhysicalexamAsync(physicalexam ret, AppUser cu)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;
            if (ret.Id == 0)
            {
                // Get all RegNo values, extract numeric part, find max
                isNew = true;
                ret.ExtraValue1 = "Saved";
                _unitOfWork.Repository<physicalexam>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<physicalexam>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<physicalexam>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<physicalexam>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<medications> SaveMedicationsAsync(medications ret, AppUser cu)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;
            if (ret.Id == 0)
            {
                // Get all RegNo values, extract numeric part, find max
                isNew = true;
                ret.ExtraValue1 = "Saved";
                _unitOfWork.Repository<medications>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<medications>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<medications>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<medications>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<family> SaveFamilyHistoryAsync(family ret, AppUser cu)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;

            if (ret.Id == 0)
            {
                ret.ExtraValue1 = "Saved";
                isNew = true;
                _unitOfWork.Repository<family>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<family>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<family>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<family>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<systemeticexam> SavesystemicExamAsync(systemeticexam ret, AppUser cu)
        {
            bool isNew = false; ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;

            if (ret.Id == 0)
            {
                ret.ExtraValue1 = "Saved";
                isNew = true;
                _unitOfWork.Repository<systemeticexam>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<systemeticexam>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<systemeticexam>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<systemeticexam>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<patient> UpdatePatientHistoryAsync(patient ret,AppUser cu)
        {
            bool isNew = false; ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;

            if (ret.Id == 0)
            {
                // Get all RegNo values, extract numeric part, find max
                isNew = true;
                _unitOfWork.Repository<patient>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<patient>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<patient>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<patient>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<List<investigations>> SaveInvestigationsAsync(List<investigations> ret,AppUser cu)
        {
            foreach (var item in ret)
            {
                item.CreatedOn = DateTime.UtcNow;               
                item.CreatedById = cu.OfficeUserId;
                item.CreatedByName = cu.UserName + "-" + cu.DisplayName;

                if (item.Id == 0)
                {
                    _unitOfWork.Repository<investigations>().Add(item);
                    item.ExtraValue1 = "Saved";
                }
                else
                    _unitOfWork.Repository<investigations>().Update(item);
            }

            try
            {
                var res = await _unitOfWork.Repository<investigations>().Complete();
                if (res <= 0)
                {
                    ret.First().AddErrorMessage("Unable to save investigations");
                }
            }
            catch (Exception ex)
            {
                ret.First().AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<additionalreports> SaveAdditionalReportsAsync(additionalreports ret,AppUser cu)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;

            if (ret.Id == 0)
            {
                ret.ExtraValue1 = "Saved";
                isNew = true;
                _unitOfWork.Repository<additionalreports>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<additionalreports>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<medications>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<additionalreports>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<pasthistory> SavePastHistoryAsync(pasthistory ret,AppUser cu)
        {
            bool isNew = false; 
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;

            if (ret.Id == 0)
            {
                ret.ExtraValue1 = "Saved";
                isNew = true;
                _unitOfWork.Repository<pasthistory>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<pasthistory>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<pasthistory>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<pasthistory>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<physicalgen> SavePhysicalgeneralAsync(physicalgen ret,AppUser cu)
        {
            bool isNew = false; 
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.CreatedById = cu.OfficeUserId;
            ret.CreatedByName = cu.UserName + "-" + cu.DisplayName;

            if (ret.Id == 0)
            {
                ret.ExtraValue1 = "Saved";
                isNew = true;
                _unitOfWork.Repository<physicalgen>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<physicalgen>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<physicalgen>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<physicalgen>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<string> GenerateNewRegNo()
        {
            var repo = _unitOfWork.Repository<patient>();

            // Get max numeric prefix from RegNo
            int maxNum = await repo.MaxNumericPrefixFromStringFieldAsync(p => p.RegNo, p => !p.IsDeleted);

            // Get the last RegNo to extract current suffix character
            string? lastRegNo = repo
                .GetSelectColumns(p => p.RegNo, p => !p.IsDeleted)
                .OrderByDescending(x => x)
                .FirstOrDefault();

            char suffix = 'C'; // Default starting character

            if (!string.IsNullOrEmpty(lastRegNo) && lastRegNo.Contains('-'))
            {
                var parts = lastRegNo.Split('-');
                if (parts.Length == 2 && parts[1].Length == 1)
                {
                    suffix = parts[1][0];
                }
            }

            // Rollover logic: reset number and increment suffix
            if (maxNum >= 9999)
            {
                maxNum = 0;
                suffix = suffix == 'Z' ? 'A' : (char)(suffix + 1);
            }

            return $"{maxNum + 1}-{suffix}";
        }

        public async Task<bool> SaveUploadPatientAsync(IReadOnlyList<patient> patients, AppUser au)
        {
            foreach (var t in patients)
            {
                var tx = await SavePatientAsync(t);
            }

            return true;

        }
        public async Task<appointments> SaveAppointmentAsync(appointments ret)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            if (ret.Id == 0)
            {
                ret.ExtraValue1 = "Save";
                isNew = true;
                _unitOfWork.Repository<appointments>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<appointments>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<appointments>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<patient>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }
        public async Task<appointmentMilestone> SaveAppointmentMilestoneAsync(appointmentMilestone ret)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            if (ret.Id == 0)
            {
                ret.ExtraValue1 = "Save";
                isNew = true;
                _unitOfWork.Repository<appointmentMilestone>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<appointmentMilestone>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<appointmentMilestone>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<appointmentMilestone>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<IReadOnlyList<appointments>> GetAppointmentByPatientIdAsync( int patient_id)
        {
            var appointments = await _unitOfWork.Repository<appointments>()
                        .GetEntityListWithSpec(new BaseSpecification<appointments>(x => x.patient_id == patient_id)
                        );
            return appointments;
        }
        public async Task<appointments> UpdateRetrieverAppointmentAsync(appointments ret,AppUser cu)//when retriever clicks done
        {
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            ret.retId = cu.OfficeUserId;
            ret.CreatedByName= cu.UserName + "-" + cu.DisplayName;
            ret.casepaperretrieved = true;
            ret.casepaperretrievaltime = DateTime.UtcNow;
            
            _unitOfWork.Repository<appointments>().Update(ret);

            try
            {
                var res = await _unitOfWork.Repository<appointments>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<appointments>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;

        }
        public async Task<IReadOnlyList<appointments>> GetAppointmentsForRetrieversAsync(AppUser appUser)
        {
            var listofappointments = await _unitOfWork.Repository<appointments>()
                        .GetEntityListWithSpec(new BaseSpecification<appointments>(x => x.status == "A" && x.casepaperretrieved == false && x.IsActive==true)
                        );
            return listofappointments;
        }
        public async Task<IReadOnlyList<appointments>> GetAppointmentsAsync(AppUser appUser)
        {
            var r = await _unitOfWork.Repository<appointments>().ListAllAsync();
            r = r.OrderBy(x => x.patient_id).ToList();
            r = r.Where(x => x.IsActive == true).ToList();
            return r;
        }
        public async Task<appointments> ValidateAppointmentAsync(appointments ret, AppUser au)
        {
            appointments obj;
            if (ret.UCode == Guid.Empty)
            {

                obj = ret;
                obj.Id = 0;
                obj.UCode = Guid.NewGuid();
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.category = "FU";
                obj.status = "A";
                //  Check for Duplicate
                var dup = await _unitOfWork.Repository<appointments>()
                        .GetEntityWithSpec(new BaseSpecification<appointments>(x => x.Id == ret.Id)
                        );
                if (dup != null)
                {
                    ret.AddErrorMessage("Appointment already exists !!");
                    return ret;
                }
            }
            else
            {
                obj = await _unitOfWork.Repository<appointments>()
                        .GetEntityWithSpec(new BaseSpecification<appointments>(x => x.UCode == ret.UCode && x.status == "A"));
                if (obj.category == null)
                    obj.category = "FU";

                if (obj == null)
                {
                    ret.AddErrorMessage("Appointment cannot be edited");
                    return ret;
                }
            }
            obj.patient_id = ret.patient_id;
            obj.category = ret.category;
            obj.assistantDoctorId = ret.assistantDoctorId;
            obj.visit_date = ret.visit_date;
            obj.IsActive = ret.IsActive;

            return obj;

        }
        public async Task<appointmentMilestone> ValidateAppointmentMilestoneAsync(appointmentMilestone ret, AppUser au)
        {
            if (ret.UCode == Guid.Empty)
            {
                var obj = ret;
                obj.Id = 0;
                obj.UCode = Guid.NewGuid();
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = au.UserName + "-" + au.DisplayName;
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;
                obj.appointmentId = ret.appointmentId;
                obj.milestone = ret.milestone;
                obj.milestoneTime = ret.milestoneTime;

                // Check for Duplicate
                var dup = await _unitOfWork.Repository<appointmentMilestone>()
                    .GetEntityWithSpec(new BaseSpecification<appointmentMilestone>(
                        x => x.appointmentId == ret.appointmentId && x.milestone == ret.milestone));

                if (dup != null)
                {
                    ret.AddErrorMessage("Appointment Status already exists !!");
                    return ret;
                }

                return obj;
            }

            // If ret.UCode is not empty, just return as is (or apply other logic)
            return ret;
        }
        public async Task<IImportExcelData<appointments>> BulkValidateAppointmentAsync(IImportExcelData<appointments> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            //if (au == null)
            //{
            //    ret.AddErrorMessage("Unknown User");
            //    return ret;
            //}

            var dupsrc = await _unitOfWork.Repository<appointments>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].patient_id = ret.DataSource[row].patient_id;
                ret.DataSource[row].category = ret.DataSource[row].category;
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.patient_id == ret.DataSource[row].patient_id).Any())
                {
                    ret.AddErrorMessage("Patient", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.patient_id == ret.DataSource[row].patient_id).Count() > 1)
                {
                    ret.AddErrorMessage("Patient", "Duplicate in List", row);
                }
            }
            return ret;

        }
        public async Task<IImportExcelData<patient>> BulkValidatePatientAsync(IImportExcelData<patient> ret, AppUser au)
        {
            if (ret.DataSource == null || ret.DataSource.Count <= 0)
            {
                ret.AddErrorMessage("Empty Data");
                return ret;
            }

            //if (au == null)
            //{
            //    ret.AddErrorMessage("Unknown User");
            //    return ret;
            //}

            var dupsrc = await _unitOfWork.Repository<patient>().ListAllAsync();

            ExcelUploadError eue = new ExcelUploadError();

            //  Verify the uploaded data
            for (int row = 0; row < ret.DataSource.Count; ++row)
            {
                ret.DataSource[row].full_name = ret.DataSource[row].full_name;
                ret.DataSource[row].mobileNo = ret.DataSource[row].mobileNo;
                ret.DataSource[row].CreatedById = au.OfficeUserId;
                ret.DataSource[row].CreatedByName = au.UserName + "-" + au.DisplayName;
                ret.DataSource[row].CreatedOn = DateTime.UtcNow;
                ret.DataSource[row].IsDeleted = false;
                ret.DataSource[row].UCode = Guid.NewGuid();
                ret.DataSource[row].Id = 0;

                if (dupsrc != null && dupsrc.Count > 0 && dupsrc.Where(x => x.mobileNo == ret.DataSource[row].mobileNo).Any())
                {
                    ret.AddErrorMessage("Patient", "Duplicate, already exists", row);
                }

                if (ret.DataSource.Where(x => x.mobileNo == ret.DataSource[row].mobileNo).Count() > 1)
                {
                    ret.AddErrorMessage("Patient", "Duplicate in List", row);
                }
            }
            return ret;
        }
        public async Task<IReadOnlyList<SessionSetup>> GetSessionsAsync()
        {
            var r = await _unitOfWork.Repository<SessionSetup>().ListAllAsync();
            r = r.OrderBy(x => x.Id).ToList();
            r = r.Where(x => x.SessionDate == DateTime.UtcNow).ToList();
            return r;
        }
        public async Task<SessionSetup> ValidateSessionAsync(SessionSetup ret, AppUser au)
        {
            SessionSetup obj;

            if (ret.UCode == Guid.Empty)
            {
                obj = ret;
                obj.Id = 0;
                obj.UCode = Guid.NewGuid();
                obj.CreatedById = au.OfficeUserId;
                obj.CreatedByName = $"{au.UserName}-{au.DisplayName}";
                obj.CreatedOn = DateTime.UtcNow;
                obj.IsDeleted = false;

                var startOfDay = DateTime.UtcNow.Date;
                var endOfDay = startOfDay.AddDays(1);

                var existingSessions = await _unitOfWork.Repository<SessionSetup>()
                    .ListAsync(new BaseSpecification<SessionSetup>(
                        x => x.SessionDate >= startOfDay &&
                             x.SessionDate < endOfDay &&
                             !x.IsDeleted && x.IsActive));

                foreach (var s in existingSessions)
                {
                    s.IsActive = false;
                    _unitOfWork.Repository<SessionSetup>().Update(s);
                }

                obj.IsActive = true; // new session becomes active
            }
            else
            {
                // Existing session — update
                obj = await _unitOfWork.Repository<SessionSetup>()
                    .GetEntityWithSpec(new BaseSpecification<SessionSetup>(x => x.UCode == ret.UCode));

                if (obj == null)
                {
                    ret.AddErrorMessage("Session does not exist");
                    return ret;
                }

                obj.SessionDate = ret.SessionDate;
                obj.SessionName = ret.SessionName;
                obj.IsActive = true;
            }

            return obj;
        }
        public async Task<SessionSetup> SaveSessionAsync(SessionSetup ret)
        {
            bool isNew = false;
            ret.CreatedOn = ret.CreatedOn.ToUniversalTime();
            if (ret.Id == 0)
            {
                isNew = true;
                _unitOfWork.Repository<SessionSetup>().Add(ret);
            }
            else
            {
                _unitOfWork.Repository<SessionSetup>().Update(ret);
            }
            try
            {
                var res = await _unitOfWork.Repository<SessionSetup>().Complete();
                if (res <= 0)
                {
                    ret.AddErrorMessage("Unable to Save");
                }
                else
                {
                    await _unitOfWork.Repository<SessionSetup>().Complete();

                }
            }
            catch (Exception ex)
            {
                ret.AddErrorMessage("Exception: " + ex.Message);
            }

            return ret;
        }

        public async Task<SessionSetup> GetActiveSessionAsync()
        {
            var ar = await _unitOfWork.Repository<SessionSetup>().GetEntityWithSpec(new BaseSpecification<SessionSetup>(x => x.IsActive == true && x.SessionDate == DateTime.UtcNow));
            return ar;
        }
        public async Task<IReadOnlyList<SessionDoctors>> GetSessionDoctorsAsync(int id)
        {
            var r = await _unitOfWork.Repository<SessionDoctors>().ListAllAsync();
            r = r.OrderBy(x => x.DoctorId).ToList();
            r = r.Where(x => x.SessionId == id).ToList();
            return r;
        }
        public async Task<IReadOnlyList<SessionDispenseTeam>> GetSessionDispenseTeamAsync( int id)
        {
            var r = await _unitOfWork.Repository<SessionDispenseTeam>().ListAllAsync();
            r = r.OrderBy(x => x.MemberId).ToList();
            r = r.Where(x => x.SessionId == id).ToList();
            return r;
        }
        public async Task<IEnumerable<SessionDoctors>> ValidateSessionDoctorsAsync(IEnumerable<SessionDoctors> retList, AppUser au)
        {
            var validatedList = new List<SessionDoctors>();

            foreach (var item in retList)
            {
                // Validate if the session is active
                var session = await _unitOfWork.Repository<SessionSetup>()
                    .GetEntityWithSpec(new BaseSpecification<SessionSetup>(x =>
                        x.Id == item.SessionId && !x.IsDeleted && x.IsActive));

                if (session == null)
                {
                    // Optional: Skip or collect error
                    item.AddErrorMessage($"Session with ID {item.SessionId} is not active or doesn't exist.");
                    continue; // Skip processing this item
                }

                var obj = item;

                if (item.UCode == Guid.Empty)
                {
                    obj.Id = 0;
                    obj.UCode = Guid.NewGuid();
                    obj.CreatedById = au?.OfficeUserId ?? 1; // fallback if au is null
                    obj.CreatedByName = au != null ? $"{au.UserName}-{au.DisplayName}" : "Admin";
                    obj.CreatedOn = DateTime.UtcNow;
                    obj.IsDeleted = false;
                }

                validatedList.Add(obj);
            }

            return validatedList;
        }

        public async Task<IEnumerable<SessionDoctors>> SaveSessionDoctorsAsync(IEnumerable<SessionDoctors> doctors)
        {
            var savedDoctors = new List<SessionDoctors>();

            foreach (var doctor in doctors)
            {
                try
                {
                    doctor.CreatedOn = doctor.CreatedOn.ToUniversalTime();

                    if (doctor.Id == 0)
                    {
                        // New entity
                        _unitOfWork.Repository<SessionDoctors>().Add(doctor);
                    }
                    else
                    {
                        // Existing entity
                        _unitOfWork.Repository<SessionDoctors>().Update(doctor);
                    }

                    var result = await _unitOfWork.Repository<SessionDoctors>().Complete();

                    if (result <= 0)
                    {
                        doctor.AddErrorMessage("Unable to save doctor with Id: " + doctor.Id);
                    }
                    else
                    {
                        savedDoctors.Add(doctor);
                    }
                }
                catch (Exception ex)
                {
                    doctor.AddErrorMessage($"Exception for doctor with Id {doctor.Id}: {ex.Message}");
                }
            }

            return savedDoctors;
        }
        public async Task<IEnumerable<SessionDispenseTeam>> ValidateSessionDispenseTeamAsync(IEnumerable<SessionDispenseTeam> team, AppUser au)
        {
            var validatedList = new List<SessionDispenseTeam>();

            foreach (var item in team)
            {
                // ✅ Check if the session is active
                var session = await _unitOfWork.Repository<SessionSetup>()
                    .GetEntityWithSpec(new BaseSpecification<SessionSetup>(x =>
                        x.Id == item.SessionId && !x.IsDeleted && x.IsActive));

                if (session == null)
                {
                    item.AddErrorMessage($"Session with ID {item.SessionId} is not active or does not exist.");
                    continue;
                }

                var obj = item;

                if (item.UCode == Guid.Empty)
                {
                    obj.Id = 0;
                    obj.UCode = Guid.NewGuid();
                    obj.CreatedById = au?.OfficeUserId ?? 1; // fallback if au is null
                    obj.CreatedByName = au != null ? $"{au.UserName}-{au.DisplayName}" : "Admin";
                    obj.CreatedOn = DateTime.UtcNow;
                    obj.IsDeleted = false;
                }
                else
                {
                    var existing = await _unitOfWork.Repository<SessionDispenseTeam>()
                        .GetEntityWithSpec(new BaseSpecification<SessionDispenseTeam>(x =>
                            x.MemberId == item.MemberId && x.SessionId == item.SessionId && !x.IsDeleted));

                    if (existing != null)
                    {
                        item.AddErrorMessage("Team member already exists in this session.");
                        continue;
                    }
                }

                // ✅ Copy remaining properties
                obj.SessionId = item.SessionId;
                obj.MemberId = item.MemberId;
                obj.IsActive = item.IsActive;

                validatedList.Add(obj);
            }

            return validatedList;
        }
        public async Task<IEnumerable<SessionDispenseTeam>> SaveSessionDispenseTeamAsync(IEnumerable<SessionDispenseTeam> members)
        {
            var savedDispenseTeam = new List<SessionDispenseTeam>();

            foreach (var dispense in members)
            {
                try
                {
                    dispense.CreatedOn = dispense.CreatedOn.ToUniversalTime();

                    if (dispense.Id == 0)
                    {
                        // New entity
                        _unitOfWork.Repository<SessionDispenseTeam>().Add(dispense);
                    }
                    else
                    {
                        // Existing entity
                        _unitOfWork.Repository<SessionDispenseTeam>().Update(dispense);
                    }

                    var result = await _unitOfWork.Repository<SessionDispenseTeam>().Complete();

                    if (result <= 0)
                    {
                        dispense.AddErrorMessage("Unable to save dispense team member with Id: " + dispense.Id);
                    }
                    else
                    {
                        savedDispenseTeam.Add(dispense);
                    }
                }
                catch (Exception ex)
                {
                    dispense.AddErrorMessage($"Exception for Dispense team memeber with Id {dispense.Id}: {ex.Message}");
                }
            }
            return savedDispenseTeam;
        }
        public async Task<IEnumerable<AppUser>> GetDoctorsAsync()
        {
            var doctors = await _userManager.Users
        .Where(u => u.AppRoleCode == "DOC" || u.AppRoleCode == "ADOC")
        .ToListAsync();

            return doctors;
        }
        public async Task<IEnumerable<AppUser>> GetDispenseTeamAsync()
        {
            List<AppUser> list = new List<AppUser>();
            var team = await _userManager.Users
        .Where(u => u.AppRoleCode == "DIS")
        .ToListAsync();

            return team;
        }
        public async Task<IEnumerable<appointments>> GetAppointmentsForDoctor(int userId, string approle)
        {
            // Ensure this is a doctor role
            if (!approle.ToLower().Contains("doc"))
            {
                return Enumerable.Empty<appointments>();
            }

            // Get the doctor user
            var doctor = await _userManager.Users.FirstOrDefaultAsync(u => u.OfficeUserId == userId);

            if (doctor == null || doctor.OfficeUserId == null)
            {
                return Enumerable.Empty<appointments>();
            }

            int doctorId = doctor.OfficeUserId;

            // Get patients assigned to this doctor
            var patients = await _unitOfWork.Repository<patient>()
                .ListAsync(new BaseSpecification<patient>(p => p.DoctorId == doctorId));

            var patientIds = patients.Select(p => p.Id).ToList();

            var appointments = await _unitOfWork.Repository<appointments>()
         .ListAsync(new BaseSpecification<appointments>(a => patientIds.Contains(a.patient_id) && a.assistantDoctorId==0 && a.IsActive==true));
            
            return appointments;
        }
        Task<OfficeUser> IMastersService.GetOfficeUser(AppUser appUser)
        {
            throw new NotImplementedException();
        }
        Task<OfficeUser> IMastersService.GetOfficeUserByLoginId(string LoginId)
        {
            throw new NotImplementedException();
        }
        Task<OfficeUser> IMastersService.GetOfficeUserByCode(string OfficeUserCode)
        {
            throw new NotImplementedException();
        }
        Task<OfficeUser> IMastersService.ValidateOfficeUserAsync(OfficeUser ret, AppUser au)
        {
            throw new NotImplementedException();
        }
        Task<IImportExcelData<OfficeUser>> IMastersService.BulkValidateOfficeUserAsync(IImportExcelData<OfficeUser> ret, AppUser au)
        {
            throw new NotImplementedException();
        }
        Task<OfficeUser> IMastersService.SaveOfficeUser(OfficeUser officeUser, AppUser au)
        {
            throw new NotImplementedException();
        }
        Task<bool> IMastersService.SaveUploadOfficeUser(IReadOnlyList<OfficeUser> officeUsers, AppUser au)
        {
            throw new NotImplementedException();
        }

    }

}
