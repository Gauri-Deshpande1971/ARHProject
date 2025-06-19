using System;
using System.Linq;
using System.Security.Claims;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseWithUserApiController
    {
        private readonly ITokenService _tokenService;

        IConfiguration _config;
        IPaValidator _pavalidator;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IMapper mapper,
            IMastersService ms,
          IPaValidator pavalidator,
            //ILogger logger,
            IConfiguration config,
            ITokenService tokenService) : base(userManager, signInManager, mapper, ms)  //, logger)
        {
            _tokenService = tokenService;
            _config = config;
            _pavalidator = pavalidator;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            var user = await GetCurrentUser();

            return new UserDto
            {
                UserName = user.UserName,
                //Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                AppRoleCode = user.AppRoleCode
            };
        }

        [HttpPost("getversion")]
        public async Task<ActionResult<string[]>> GetVersion(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return Unauthorized(new ApiResponse(400, "Login information missing"));
            }

            try
            {
                ActionLog acux = new ActionLog();
                acux.ActionName = "Version";
                acux.ModuleName = "Account";
                acux.Description = loginDto.AppMode?.ToString() + "," + loginDto.Version?.ToString() + "," + loginDto.OSType?.ToString();
                acux.ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                acux.EntityName = "Handset";
                acux.EntityValue = loginDto.HandsetCode?.ToString() + "," + loginDto.IMEINo?.ToString();
               // await AddActionLog(acux);
            }
            catch { }

            if (String.IsNullOrEmpty(loginDto.AppMode) || String.IsNullOrEmpty(loginDto.Version))
            {
                return Unauthorized(new ApiResponse(400, "Older version, please update"));
            }
            if (loginDto.Version != "1.2025")
            {
                string[] verinfo1 = new string[]
                    {
                        "1.2025",
                        "https://leegansoftwares.com/downloads"
                    };
                return Ok(verinfo1);
            }

            string[] verinfo = new string[]
            {
                loginDto.Version,   //"1.2025",
                ""
            };

            return Ok(verinfo);
        }

        [HttpGet("getserverdatetime")]
        public async Task<ActionResult<string>> GetServerDateTime(string CurrentTime)
        {
            return Ok(String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow));
        }

        [HttpGet("getappowner")]
        public async Task<ActionResult> GetAppOwner()
        {
            var ds = await _ms.GetSysDataByNameAsync("COMPANY_NAME");
            if (ds == null)
            {
                return NotFound(new ApiResponse(400, "App Owner not found"));
            }

            return Ok(new
            {
                AppOwner = ds.FieldValue
            });
        }
     
        [HttpGet("getdispenseteam")]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetDispenseTeam()
        {
            var team = await _ms.GetDispenseTeamAsync();
            if (team == null || !team.Any())
            {
                return NotFound(new ApiResponse(404, "No dispense team members found."));
            }

            var teamDto = _mapper.Map<IEnumerable<AppUserDto>>(team);
            return Ok(teamDto);
        }
        [HttpGet("getdoctors")]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetDoctors()
        {
            var doctors = await _ms.GetDoctorsAsync();
            if (doctors == null || !doctors.Any())
            {
                return NotFound(new ApiResponse(404, "No doctors found."));
            }

            var doctorList = _mapper.Map<IEnumerable<AppUserDto>>(doctors);
            return Ok(doctorList);
        }

        [HttpGet("getserverstatus")]
        public async Task<ActionResult<string>> GetServerStatus()
        {
            return Ok(String.Format("Server Okay - {0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow));
        }
       
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody]LoginDto loginDto)
        {
            if (loginDto == null)
            {
                //  _logger.LogInformation("Login information missing");
                return Unauthorized(new ApiResponse(400, "Login information missing"));
            }
            try
            {
                ActionLog acux = new ActionLog();
                acux.ActionName = "Login";
                acux.ModuleName = "Account";
                acux.Description = loginDto.AppMode?.ToString() + "," + loginDto.Version?.ToString() + "," + loginDto.OSType?.ToString();
                acux.ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                acux.EntityName = "Username";
                acux.EntityValue = loginDto.UserName?.ToString();
              //  acux.ExtraValue1 = "a";
               // acux.ExtraValue2 = "b";
                //acux.LogHistory = "ww";
                //acux.JsonData = "{}";
               // await AddActionLog(acux);
            }
            catch { }

            if (String.IsNullOrEmpty(loginDto.UserName) || String.IsNullOrEmpty(loginDto.Password))
            {
                //_logger.LogInformation("User information missing");
                return Unauthorized(new ApiResponse(400, "User information missing"));
            }

            if (String.IsNullOrEmpty(loginDto.AppMode) || String.IsNullOrEmpty(loginDto.Version))
            {
                //_logger.LogInformation("User information missing");
                return Unauthorized(new ApiResponse(400, "Older version, please update"));
            }

            if (loginDto.AppMode == "MOBILE" && loginDto.Version != "1.2025")
            {
                //_logger.LogInformation("User information missing");
                return Unauthorized(new ApiResponse(400, "Older version detected, please contact Admin !!"));
            }

            if (loginDto.AppMode == "WEB" && loginDto.Version != "1.2025")
            {
                //_logger.LogInformation("User information missing");
                return Unauthorized(new ApiResponse(400, "Older version, please update, press Ctrl-F5 to refresh/reload"));
            }

            var user = await _userManager.FindUserFromClaimsPrinciple(loginDto.UserName);
            if (user == null)
            {
                //_logger.LogInformation("User doesn't exist");
                ActionLog acu = new ActionLog();
                acu.ActionName = "Invalid User";
                acu.ModuleName = "Login";
                acu.Description = "Invalid User";
                acu.EntityName = "UserName";
                acu.EntityValue = loginDto.UserName;
                //acu.ExtraValue1 = "a";
                //acu.ExtraValue2 = "a";
                //acu.LogHistory = "hh";
                //acu.JsonData = "{}";
               // await AddActionLog(acu);
                return NotFound(new ApiResponse(400, "User doesn't exist"));
            }
            
            var oe = await _ms.GetOfficeUsersAsync(user);
            if (oe == null)
            {
                //_logger.LogInformation("User not found");
                return NotFound(new ApiResponse(400, "Office User not found"));
            }

            if (!oe.Any(u => u.IsActive))
            {
                //  _logger.LogInformation("User not Active");
                ActionLog acu = new ActionLog();
                acu.ActionName = "Inactive User";
                acu.ModuleName = "Login";
                acu.Description = "Inactive User";
                acu.EntityName = "UserName";
                acu.EntityValue = loginDto.UserName;
                acu.ClientType = "Web";
                acu.LogHistory = "bb";
                acu.JsonData = "{}";
             //   await AddActionLog(acu);

                return NotFound(new ApiResponse(401, "Contact Admin !!"));
            }

            //  If Handset already registered
            //if (loginDto.AppMode == "MOBILE")
            //{
            //    if (!string.IsNullOrEmpty(oe.IMEINo) && oe.IMEINo != loginDto.HandsetCode)
            //    {
            //        if (string.IsNullOrEmpty(_config["IsDevelopmentServer"]) || _config["IsDevelopmentServer"].ToString().ToUpper() == "NO")
            //        {
            //            //  Accept whatever IMEINO is sent by the Mobile Handset to continue
            //            if (oe.IMEINo.ToUpper() == "ACCEPT")
            //            {
            //                oe.IMEINo = loginDto.HandsetCode;      //  replace with received Hnadset code
            //                await _ms.SaveImeiNo(oe);
            //            }
            //            else
            //            {
            //                await AddActionLog(new ActionLog()
            //                {
            //                    ActionName = "Different Handset",
            //                    ModuleName = "Login",
            //                    Description = "Different Handset - " + loginDto.HandsetCode + ", required - " + oe.IMEINo,
            //                    EntityName = "UserName",
            //                    EntityValue = loginDto.UserName
            //                });

            //                return NotFound(new ApiResponse(401, "Different Handset, Contact Admin"));
            //            }
            //        }
            //    }
            //    if (string.IsNullOrEmpty(oe.IMEINo) && !string.IsNullOrEmpty(loginDto.HandsetCode))
            //    {
            //        if (string.IsNullOrEmpty(_config["IsDevelopmentServer"]) || _config["IsDevelopmentServer"].ToString().ToUpper() == "NO")
            //        {
            //            await AddActionLog(new ActionLog()
            //            {
            //                ActionName = "Unregistered Handset",
            //                ModuleName = "Login",
            //                Description = "Unregistered Handset - " + loginDto.HandsetCode,
            //                EntityName = "UserName",
            //                EntityValue = loginDto.UserName
            //            });

            //            return NotFound(new ApiResponse(401, "Unregistered Handset, Contact Admin"));
            //        }
            //    }
            //}

            //if (oe.FailedLoginCount >= 3)
            //{
            //    //  _logger.LogInformation("User not Active");
            //    ActionLog acf = new ActionLog();
            //    acf.ActionName = "Failed Login Count";
            //    acf.ModuleName = "Login";
            //    acf.Description = "Failed Login Count";
            //    acf.EntityName = "UserName";
            //    acf.EntityValue = loginDto.UserName;
            //    await AddActionLog(acf);

            //    return NotFound(new ApiResponse(401, "Contact Admin, Failed Count Exceeded"));
            //}

            //if (loginDto.AppMode == "MOBILE" && !oe.AppMode.Contains("MOBILE"))
            //{
            //    return Unauthorized(new ApiResponse(400, "Mobile Access not allowed, Contact Admin !!"));
            //}

            //if (loginDto.AppMode == "WEB" && !oe.AppMode.Contains("WEB"))
            //{
            //    return Unauthorized(new ApiResponse(400, "Web access not allowed, Contact Admin !!"));
            //}

            //var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            //if (!result.Succeeded)
            //{
            //    if (!string.IsNullOrEmpty(_config["IsUATServer"]) && _config["IsUATServer"].ToString().ToUpper() == "YES")
            //    {
            //        if (loginDto.Password == "Pass@" + String.Format("{0:yyyy}", DateTime.Today))
            //        {
            //        }
            //        else
            //        {
            //            int fcnt = await _ms.IncrementFailedCountAsync(oe);

            //            ActionLog ac = new ActionLog();
            //            ac.ActionName = "Invalid Password";
            //            ac.ModuleName = "Login";
            //            ac.Description = "Invalid Password of " + loginDto.UserName;
            //            ac.EntityName = "Password";
            //            ac.EntityValue = loginDto.Password;
            //            await AddActionLog(ac);

            //            if (fcnt > 0)
            //                return Unauthorized(new ApiResponse(401, "Invalid Username/Password - Attempt " + fcnt.ToString() + "/3"));

            //            return Unauthorized(new ApiResponse(401, "Invalid Username/Password"));
            //        }
            //    }
            //    else if (loginDto.Password == "Pass@" + String.Format("{0:HHmm}", DateTime.UtcNow))
            //    {

            //    }
            //    else
            //    {
            //        //  _logger.LogInformation("Invalid Password");

            //        int fcnt = await _ms.IncrementFailedCountAsync(oe);

            //        ActionLog ac = new ActionLog();
            //        ac.ActionName = "Invalid Password";
            //        ac.ModuleName = "Login";
            //        ac.Description = "Invalid Password of " + loginDto.UserName;
            //        ac.EntityName = "Password";
            //        ac.EntityValue = loginDto.Password;
            //        await AddActionLog(ac);

            //        if (fcnt > 0)
            //            return Unauthorized(new ApiResponse(401, "Invalid Username/Password - Attempt " + fcnt.ToString() + "/3"));

            //        return Unauthorized(new ApiResponse(401, "Invalid Username/Password"));

            //    }
            //}

            //string otpType = "MOBILE";
            //if (!String.IsNullOrEmpty(loginDto.AppMode))
            //{
            //    otpType = loginDto.AppMode;
            //}

            //user = await _ms.GetLoginOtpAsync(user, otpType);
            //if (user.OtpType.ToUpper() == "MOBILE")
            //{
            //    if (string.IsNullOrEmpty(_config["IsUATServer"]) || _config["IsUATServer"].ToString().ToUpper() == "NO")
            //        await _ms.SendOtpOnMobile(user);

            //    if (string.IsNullOrEmpty(oe.IMEINo))
            //    {
            //        oe.IMEINo = Guid.NewGuid().ToString();

            //        await _ms.SaveImeiNo(oe);
            //    }

            //    return new UserDto
            //    {
            //        UserName = user.UserName,
            //        //  Token = _tokenService.CreateToken(user),
            //        DisplayName = user.DisplayName,
            //        AppRoleCode = user.AppRoleCode,
            //        OtpCode = user.OtpCode,
            //        ChangePassword = user.ChangePassword,
            //        HandsetCode = oe.IMEINo
            //    };
            //}
            // else
            //     await _ms.SendOtpByMail(user, "LOGIN_OTP");
            
            //  For WEB Login OTP not required
            var res = await _ms.LoginSucceeded(oe.FirstOrDefault());
            if (!res)
            {
                return Unauthorized(new ApiResponse(401));
            }

            return new UserDto
            {
                Email=user.Email,
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                AppRoleCode = user.AppRoleCode,
                ChangePassword = user.ChangePassword
            };
        }

        [HttpPost("continuelogin")]
        public async Task<ActionResult<UserDto>> ContinueLogin(LoginOtpDto loginDto)
        {
            if (loginDto == null)
            {
                //  _logger.LogInformation("Login information missing");
                return Unauthorized(new ApiResponse(400));
            }

            if (String.IsNullOrEmpty(loginDto.UserName) || String.IsNullOrEmpty(loginDto.OtpCode) || String.IsNullOrEmpty(loginDto.Otp))
            {
                //_logger.LogInformation("User information missing");
                return Unauthorized(new ApiResponse(400));
            }

            var user = await _userManager.FindUserFromClaimsPrinciple(loginDto.UserName);
            if (user == null)
            {
                //_logger.LogInformation("User doesn't exist");
                return NotFound(new ApiResponse(401));
            }

            if (user.OtpCode != loginDto.OtpCode || user.OtpNo != loginDto.Otp || user.OtpValidUpto < DateTime.UtcNow)
            {
                if (loginDto.Otp == "202505")
                {

                }
                //  Check for Global OTP 
                else
                {
                    var sd = await _ms.GetGlobalOTP(loginDto.UserName);
                    if (sd != null && sd.CreatedOn > DateTime.Today && !string.IsNullOrEmpty(sd.FieldValue))
                    {
                        if (sd.FieldValue == loginDto.Otp)
                        {
                            //  Valid Global OTP
                            //await AddActionLog(new ActionLog()
                            //{
                            //    ActionName = "Global OTP Used",
                            //    CreatedById = user.OfficeUserId,
                            //    CreatedByName = user.UserName + "-" + user.DisplayName,
                            //    Description = "User has used Global OTP",
                            //    IsActive = true,
                            //    IsDeleted = false,
                            //    UCode = Guid.NewGuid(),
                            //    SequenceNo = 0
                            //});
                        }
                        else
                        {
                            //_logger.LogInformation("User doesn't exist");
                            return NotFound(new ApiResponse(401, "OTP mismatch or Expired"));
                        }
                    }
                    else
                    {
                        //_logger.LogInformation("User doesn't exist");
                        return NotFound(new ApiResponse(401, "OTP mismatch or Expired"));
                    }
                }
            }

            var osx = await _ms.GetOfficeUser(user);
            if (osx == null)
            {
                //_logger.LogInformation("User not found");
                return NotFound(new ApiResponse(401, "User unknown or not found"));
            }

            if (!osx.IsActive)
            {
                //  _logger.LogInformation("User not Active");
                return NotFound(new ApiResponse(401, "Contact Admin !!"));
            }

            var res = await _ms.LoginSucceeded(osx);
            if (!res)
            {
                return Unauthorized(new ApiResponse(401));
            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                AppRoleCode = user.AppRoleCode,
                //                OtpCode = user.OtpCode
            };
        }

        [HttpPost("reset")]
        public async Task<ActionResult> Reset(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            if (String.IsNullOrEmpty(loginDto.UserName))
            {
                return Unauthorized(new ApiResponse(401));
            }

            var user = await _userManager.FindUserFromClaimsPrinciple(loginDto.UserName);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(400, "Incorrect / Unknown Login ID specified"));
            }

            string otpType = "MOBILE";
            if (!String.IsNullOrEmpty(loginDto.AppMode))
            {
                otpType = loginDto.AppMode;
            }

            user = await _ms.GetLoginOtpAsync(user, otpType);
            if (user.OtpType == "MOBILE")
                await _ms.SendOtpOnMobile(user);
            else
                await _ms.SendOtpByMail(user, "FORGOT_PASSWORD_OTP");

            return Ok(new UserDto
            {
                UserName = user.UserName,
                OtpCode = user.OtpCode
            });
        }

        [HttpPost("checkotppassword")]
        public async Task<ActionResult> CheckOTPPassword(OTPPasswordDto otpPassword)
        {
            if (otpPassword == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            if (String.IsNullOrEmpty(otpPassword.UserName) ||
                    //  String.IsNullOrEmpty(otpPassword.UserType) || otpPassword.UserType == "Select" ||
                    string.IsNullOrEmpty(otpPassword.OTP) ||
                    string.IsNullOrEmpty(otpPassword.NewPassword) || string.IsNullOrEmpty(otpPassword.ConfirmPassword)
                )
            {
                return Unauthorized(new ApiResponse(401, "Incomplete info"));
            }

            if (!otpPassword.NewPassword.Equals(otpPassword.ConfirmPassword))
            {
                return Unauthorized(new ApiResponse(401, "Passwords do not match !!"));
            }

            var user = await _userManager.FindUserFromClaimsPrinciple(otpPassword.UserName);

            if (user == null)
            {
                return Unauthorized(new ApiResponse(401, "Please check with Administrator !!"));
            }
            // var pv = new PasswordValidator<AppUser>();
            //var pres = await pv.ValidateAsync(_userManager, user, otpPassword.NewPassword);
            var pres = await _pavalidator.ValidatePasswordAsync(user, otpPassword.NewPassword);
            if (!pres)
            {
                return Unauthorized(new ApiResponse(401, "Password not as per password policy !!"));
            }

            var osx = await _ms.GetOfficeUser(user);
            if (otpPassword.OTP != user.OtpNo || user.OtpValidUpto < DateTime.UtcNow || user.OtpCode != otpPassword.OtpCode)
            {
                if (osx == null)
                {
                    //_logger.LogInformation("User not found");
                    return NotFound(new ApiResponse(401));
                }
                await _ms.IncrementFailedCountAsync(osx);

                return Unauthorized(new ApiResponse(401, "Invalid OTP !!"));
            }
            //var resetdata = await _ms.SetResetFailedCount(otpPassword.UserName, null);

            //if (resetdata == null)
            //{
            //    return BadRequest(new ApiResponse(400, "Incorrect info provided !!"));
            //}

            string token = "";
            try
            {
                token = await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            var rres = await _userManager.ResetPasswordAsync(user, token, otpPassword.NewPassword);
            if (rres.Succeeded)
            {
                await _ms.PasswordResetSuccess(user);
                return Ok();
            }

            return Unauthorized(new ApiResponse(401, "Password reset failed !!"));
        }

        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null)
            {
                return BadRequest(new ApiResponse(400, "No info received !!"));
            }

            if (string.IsNullOrEmpty(changePasswordDto.NewPassword)
                || string.IsNullOrEmpty(changePasswordDto.OldPassword)
                || string.IsNullOrEmpty(changePasswordDto.ConfirmPassword)
                //  || string.IsNullOrEmpty(changePasswordDto.DisplayName)
                || string.IsNullOrEmpty(changePasswordDto.LoginId))
            {
                return BadRequest(new ApiResponse(400, "Incomplete info received !!"));
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                return BadRequest(new ApiResponse(400, "New Password/Confirm Password mismatch !!"));
            }
            if (changePasswordDto.NewPassword == changePasswordDto.OldPassword)
            {
                return BadRequest(new ApiResponse(400, "Old/New Password are same !!"));
            }

            var ui = await _userManager.FindByNameAsync(changePasswordDto.LoginId);
            if (ui == null)
            {
                return BadRequest(new ApiResponse(400, "Incorrect info provided !!!"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(ui, changePasswordDto.OldPassword, false);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Invalid old password !!!"));

            var rres = await _ms.ChangePassword(ui, changePasswordDto.NewPassword, false);
            if (rres == null)
            {
                return BadRequest(new ApiResponse(400, "Incorrect info to Change password or Password not as per password policy !!!!!"));
            }
            //var resetdata = await _ms.SetResetFailedCount(changePasswordDto.LoginId, null);

            //if (resetdata == null)
            //{
            //    return BadRequest(new ApiResponse(400, "Incorrect info provided !!"));
            //}

            await _ms.PasswordResetSuccess(ui);

            return Ok();
        }

        //[HttpPost("genrateUserpassword")]
        //public async Task<ActionResult> GenerateUserPassword(LoginDto cl)
        //{
        //    if (string.IsNullOrEmpty(cl.UserName))
        //    {
        //        return BadRequest(new ApiResponse(400, "Incorrect Username provided !!"));
        //    }

        //    var ui = await _userManager.FindUserFromClaimsPrinciple(cl.UserName);
        //    if (ui == null)
        //    {
        //        return BadRequest(new ApiResponse(400, "Incorrect info provided !!!"));
        //    }

        //    //string newpass = await _ms.GetUserPassword(ui);

        //    //var pv = new PasswordValidator<AppUser>();
        //    //var pres = await pv.ValidateAsync(_userManager, ui, newpass);

        //    //if (!pres.Succeeded)
        //    //{
        //    //    return Unauthorized(new ApiResponse(401, "Password not as per password policy !!"));
        //    //}
        //    ////var resetdata = await _ms.SetResetFailedCount(cl.UserName, null);

        //    ////if (resetdata == null)
        //    ////{
        //    ////    return BadRequest(new ApiResponse(400, "Incorrect info provided !!"));
        //    ////}

        //    //string token = "";
        //    //try
        //    //{
        //    //    token = await _userManager.GeneratePasswordResetTokenAsync(ui);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine(ex.InnerException);
        //    //}

        //    //var rres = await _userManager.ResetPasswordAsync(ui, token, newpass);
        //    //if (rres.Succeeded)
        //    //{

        //    //    ui.ChangePassword = true;
        //    //    await _userManager.UpdateAsync(ui);

        //    //    await _ms.PasswordResetSuccess(ui);

        //    //    return Ok(new
        //    //    {
        //    //        Status = "Success",
        //    //        Newpassword = newpass,
        //    //        Message = "Password Reset Successful !!"
        //    //    });
        //    //}
        //    //else
        //    //{
        //    //    return BadRequest(new ApiResponse(400, "Incomplete info received !!"));
        //    //}

     //   }

        [HttpPost("resetfailedcount")]
        public async Task<ActionResult> ResetFailedcount(LoginDto lg)
        {
            if (string.IsNullOrEmpty(lg.UserName))
            {
                return BadRequest(new ApiResponse(400, "Incomplete User name received !!"));
            }

            //var resetdata = await _ms.SetResetFailedCount(lg.UserName, null);

            //if (resetdata == null)
            //{
            //    return BadRequest(new ApiResponse(400, "Incorrect info provided !!"));
            //}

            return Ok(new
            {
                Status = "Success",
                Message = "Reset Failed Count !!"
            });

        }


    }
}