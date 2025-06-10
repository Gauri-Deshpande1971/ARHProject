using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Identity;
using Core.Models;

namespace Core.Interfaces
{
    public interface IMastersService
    {
        Task<DateTime> GetToday();
        Task<DateTime> SetToday(string today);
        Task<SysData> GetGlobalOTP(string UserCode);
        Task<SysData> SaveGlobalOTP(AppUser au);
        Task<SysData> SaveGlobalOTP(string UserCode);

        Task<Boolean> LoginSucceeded(AppUser emp);
        Task<AppUser> GetLoginOtpAsync(AppUser appUser, string otpType);
        Task<IReadOnlyList<NavmenuOfUser>> GetNavmenuOfUserAsync(string appUserId);

        //  -------------------
        //  Auto Generated
        //  -------------------
        Task<IReadOnlyList<AppRole>> GetAppRolesAsync(AppUser appUser);
        Task<AppRole> GetAppRoleByNameAsync(string AppRoleName);
        Task<AppRole> GetAppRoleByCodeAsync(string UCode);
        Task<IReadOnlyList<Attachment>> GetAttachmentsAsync(AppUser appUser);
        Task<Attachment> GetAttachmentByNameAsync(string AttachmentName);
        Task<Attachment> GetAttachmentByCodeAsync(string UCode);
        Task<IReadOnlyList<FormGridHeader>> GetFormGridHeadersAsync(AppUser appUser);
        Task<FormGridHeader> GetFormGridHeaderByNameAsync(string FormGridHeaderName);
        Task<FormGridHeader> GetFormGridHeaderByCodeAsync(string UCode);
        Task<IReadOnlyList<MailConfig>> GetMailConfigsAsync(AppUser appUser);
        Task<MailConfig> GetMailConfigByNameAsync(string MailConfigName);
        Task<MailConfig> GetMailConfigByCodeAsync(string UCode);
        Task<IReadOnlyList<MailLog>> GetMailLogsAsync(AppUser appUser);
        Task<MailLog> GetMailLogByNameAsync(string MailLogName);
        Task<MailLog> GetMailLogByCodeAsync(string UCode);
        Task<IReadOnlyList<NavMenu>> GetNavMenusAsync(AppUser appUser);
        Task<NavMenu> GetNavMenuByNameAsync(string NavMenuName);
        Task<NavMenu> GetNavMenuByCodeAsync(string UCode);
        Task<IReadOnlyList<Organization>> GetOrganizationsAsync(AppUser appUser);
        Task<Organization> GetOrganizationByNameAsync(string OrganizationName);
        Task<Organization> GetOrganizationByCodeAsync(string UCode);
        Task<IReadOnlyList<SysData>> GetSysDatasAsync(AppUser appUser);
        Task<SysData> SaveSysAsync(SysData ret, AppUser au);
        Task<SysData> GetSysDataByNameAsync(string SysDataName);
        Task<SysData> GetSysDataByCodeAsync(string UCode);
        Task<IReadOnlyList<UserNavMenu>> GetUserNavMenusAsync(AppUser appUser);
        Task<UserNavMenu> GetUserNavMenuByNameAsync(string UserNavMenuName);
        Task<UserNavMenu> GetUserNavMenuByCodeAsync(string UCode);
        Task<IReadOnlyList<FormGridDetail>> GetFormGridDetailsAsync(AppUser appUser);
        Task<FormGridDetail> GetFormGridDetailByNameAsync(string FormGridDetailName);
        Task<FormGridDetail> GetFormGridDetailByCodeAsync(string UCode);


        //  --------------------
        //  AppRole
        //  --------------------
        Task<AppRole> ValidateAppRoleAsync(AppRole ret, AppUser au);
        Task<IImportExcelData<AppRole>> BulkValidateAppRoleAsync(IImportExcelData<AppRole> ret, AppUser au);
        Task<AppRole> SaveAppRoleAsync(AppRole ret);
        Task<bool> SaveUploadAppRoleAsync(IReadOnlyList<AppRole> AppRoles, AppUser au);
        //  --------------------
        //  Organization
        //  --------------------
        Task<Organization> ValidateOrganizationAsync(Organization ret, AppUser au);
        Task<IImportExcelData<Organization>> BulkValidateOrganizationAsync(IImportExcelData<Organization> ret, AppUser au);
        Task<Organization> SaveOrganizationAsync(Organization ret);
        Task<bool> SaveUploadOrganizationAsync(IReadOnlyList<Organization> Organizations, AppUser au);
        // --------------------------
        //  Rate
        // -------------------------
        Task<IReadOnlyList<Rate>> GetRatesAsync(AppUser appUser);
        Task<Rate> GetRateByTypeAsync(string type);
        Task<Rate> GetRateByCodeAsync(string UCode);
        Task<Rate> ValidateRateAsync(Rate ret, AppUser au, string type);
        Task<IImportExcelData<Rate>> BulkValidateRateAsync(IImportExcelData<Rate> ret, AppUser au);
        Task<Rate> SaveRateAsync(Rate ret);
        Task<bool> SaveUploadRateAsync(IReadOnlyList<Rate> Rates, AppUser au);
        //--------------------------------
        //Medicine
        //---------------------------------
        Task<IReadOnlyList<Medicine>> GetMedicinesAsync(AppUser appUser);        
        Task<Medicine> GetMedicineByCodeAsync(string UCode);
        Task<Medicine> ValidateMedicineAsync(Medicine ret, AppUser au);
        Task<IImportExcelData<Medicine>> BulkValidateMedicineAsync(IImportExcelData<Medicine> ret, AppUser au);
        Task<Medicine> SaveMedicineAsync(Medicine ret);
        Task<bool> SaveUploadMedicineAsync(IReadOnlyList<Medicine> Medicines, AppUser au);

        //  --------------------
        //  Attachments
        //  --------------------
        Task<IReadOnlyList<Attachment>> GetAttachmentListAsync(string EntityName, string EntityFieldName, string EntityKeyValue);
        Task<Attachment> GetAttachmentAsync(string EntityName, string EntityFieldName, string EntityKeyValue, string FileType);

        Task<bool> SaveAttachFile(Attachment attachment);

        //  --------------------
        //  Mobile SMS
        //  --------------------
        Task<AppUser> SendOtpOnMobile(AppUser user);

        //  --------------------
        //  Email
        //  --------------------
        Task<AppUser> SendOtpByMail(AppUser user, string ActionName);
        Task<bool> PasswordResetSuccess(AppUser user);
        Task<bool> ProfileCreatedMail(AppUser user, string Password, AppUser au);
        Task<bool> ChangePasswordMail(AppUser user, string Password);
        Task<AppUser> ChangePassword(AppUser currentUser, string NewPassword, bool savetoextdatasync);
        Task<bool> DeleteDocumentAsync(string PhyFilename);
        Task<MailConfig> ValidateMailConfigAsync(MailConfig ret, AppUser au);
        Task<MailConfig> SaveMailConfigAsync(MailConfig ret);
        Task<IReadOnlyList<MailLog>> GetMailLogsAsync(DateTime DtFrom, DateTime DtTo);
        Task<IReadOnlyList<ActionLog>> GetActionLog(DateTime DtFrom, DateTime DtTo);
        Task<AppRole> DeleteAppRoleAsync(AppRole ret, AppUser au);
        Task<Organization> DeleteOrganizationAsync(Organization ret, AppUser au);
        Task<AppRole> ValidateAppRoleDeleteAsync(Guid ucode, string reason, AppUser au);
        Task<Organization> ValidateOrganizationDeleteAsync(Guid ucode, string reason, AppUser au);
        Task<ActionLog> AddActionLogAsync(ActionLog actlg);
        Task<int> AddActionLogAsync(string ActionName, string Description, string EntityName = null, string EntityValue = null);
        Task<IReadOnlyList<UserNavMenu>> GetNavMenuOfUserManageAsync(string UserCode, string AppRoleCode);
    }
}
