using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string UserCode { get; set; }
        public string AppRoleCode { get; set; }
        public string AppRoleName { get; set; }
        public int UserId { get; set; }
        public string MobileNo { get; set; }

        public bool ChangePassword { get; set; }
        public DateTime? LastPasswordChange { get; set; }

        public string OtpCode { get; set; }
        public string OtpNo { get; set; }
        public DateTime? OtpValidUpto { get; set; }

        //  Type of OTP - SMS or Email
        public string OtpType { get; set; }

        // public void CopyInternalToExternal(AppUser src)
        // {
        //     Id = src.Id;
        //     DisplayName = src.DisplayName;
        //     UserCode = src.UserCode;
        //     AppRoleCode = src.AppRoleCode;
        //     AppRoleName = src.AppRoleName;
        //     UserId = src.UserId;
        //     MobileNo = src.MobileNo;
        //     ChangePassword = src.ChangePassword;
        //     LastPasswordChange = src.LastPasswordChange;
        //     OtpCode = src.OtpCode;
        //     OtpNo = src.OtpNo;
        //     OtpValidUpto = src.OtpValidUpto;
        //     OtpType = src.OtpType;
        //     UserName = src.UserName;
        //     NormalizedUserName = src.NormalizedUserName;
        //     Email = src.Email;
        //     NormalizedEmail = src.NormalizedEmail;
        //     EmailConfirmed = src.EmailConfirmed;
        //     PasswordHash = src.PasswordHash;
        //     SecurityStamp = src.SecurityStamp;
        //     ConcurrencyStamp = src.ConcurrencyStamp;
        //     PhoneNumber = src.PhoneNumber;
        //     PhoneNumberConfirmed = src.PhoneNumberConfirmed;
        //     TwoFactorEnabled = src.TwoFactorEnabled;
        //     LockoutEnd = src.LockoutEnd;
        //     LockoutEnabled = src.LockoutEnabled;
        //     AccessFailedCount = src.AccessFailedCount;            
        // }

    }
}