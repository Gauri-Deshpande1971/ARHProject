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

        Task<Boolean> LoginSucceeded(OfficeUser emp);
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
        Task<IReadOnlyList<City>> GetCitiesAsync();
        Task<City> GetCityByNameAsync(string CityName);
        Task<City> GetCityByCodeAsync(string UCode);
        Task<Country> GetCountryByNameAsync(string CountryName);
        Task<Country> GetCountryByCodeAsync(string UCode);
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
        Task<IReadOnlyList<State>> GetStatesAsync(AppUser appUser);
        Task<State> GetStateByNameAsync(string StateName);
        Task<State> GetStateByCodeAsync(string UCode);
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
        //  City
        //  --------------------
        Task<City> ValidateCityAsync(City ret, AppUser au);
        Task<IImportExcelData<City>> BulkValidateCityAsync(IImportExcelData<City> ret, AppUser au);
        Task<City> SaveCityAsync(City ret);
        Task<bool> SaveUploadCityAsync(IReadOnlyList<City> Citys, AppUser au);
        //  --------------------
        //  Country
        //  --------------------
        Task<Country> ValidateCountryAsync(Country ret, AppUser au);
        Task<IImportExcelData<Country>> BulkValidateCountryAsync(IImportExcelData<Country> ret, AppUser au);
        Task<Country> SaveCountryAsync(Country ret);
        Task<bool> SaveUploadCountryAsync(IReadOnlyList<Country> Countrys, AppUser au);
        //  --------------------
        //  Organization
        //  --------------------
        Task<Organization> ValidateOrganizationAsync(Organization ret, AppUser au);
        Task<IImportExcelData<Organization>> BulkValidateOrganizationAsync(IImportExcelData<Organization> ret, AppUser au);
        Task<Organization> SaveOrganizationAsync(Organization ret);
        Task<bool> SaveUploadOrganizationAsync(IReadOnlyList<Organization> Organizations, AppUser au);
        //  --------------------
        //  State
        //  --------------------
        Task<State> ValidateStateAsync(State ret, AppUser au);
        Task<IImportExcelData<State>> BulkValidateStateAsync(IImportExcelData<State> ret, AppUser au);
        Task<State> SaveStateAsync(State ret);
        Task<bool> SaveUploadStateAsync(IReadOnlyList<State> States, AppUser au);

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

      //  Task<bool> DeleteDocumentAsync(string PhyFilename);
        Task<MailConfig> ValidateMailConfigAsync(MailConfig ret, AppUser au);
        Task<MailConfig> SaveMailConfigAsync(MailConfig ret);
        Task<IReadOnlyList<MailLog>> GetMailLogsAsync(DateTime DtFrom, DateTime DtTo);
        Task<IReadOnlyList<ActionLog>> GetActionLog(DateTime DtFrom, DateTime DtTo);

        Task<AppRole> DeleteAppRoleAsync(AppRole ret, AppUser au);
        Task<Organization> DeleteOrganizationAsync(Organization ret, AppUser au);
        Task<Country> DeleteCountryAsync(Country ret, AppUser au);
        Task<State> DeleteStateAsync(State ret, AppUser au);
        Task<City> DeleteCityAsync(City ret, AppUser au);
        Task<AppRole> ValidateAppRoleDeleteAsync(Guid ucode, string reason, AppUser au);
        Task<Organization> ValidateOrganizationDeleteAsync(Guid ucode, string reason, AppUser au);
        Task<Country> ValidateCountryDeleteAsync(Guid ucode, string reason, AppUser au);
        Task<State> ValidateStateDeleteAsync(Guid ucode, string reason, AppUser au);
        Task<City> ValidateCityDeleteAsync(Guid ucode, string reason, AppUser au);
        Task<ActionLog> AddActionLogAsync(ActionLog actlg);
        Task<int> AddActionLogAsync(string ActionName, string Description, string EntityName = null, string EntityValue = null);
        Task<IReadOnlyList<UserNavMenu>> GetNavMenuOfUserManageAsync(string UserCode, string AppRoleCode);

        //  --------------------
        //  Department
        //  --------------------
        //  Department
        //  --------------------
        Task<Core.Entities.Department> ValidateDepartmentAsync(Department ret, AppUser au, string SubDepartmentNames);
        Task<IImportExcelData<Core.Entities.Department>> BulkValidateDepartmentAsync(IImportExcelData<Core.Entities.Department> ret, AppUser au);
        Task<Core.Entities.Department> SaveDepartmentAsync(Department ret, string SubDepartmentNames);
        Task<bool> SaveUploadDepartmentAsync(IReadOnlyList<Core.Entities.Department> Departments, AppUser au);
        Task<IReadOnlyList<Core.Entities.Department>> GetDepartmentsAsync(AppUser appUser);
        Task<Core.Entities.Department> GetDepartmentByNameAsync(string DepartmentName);
        Task<Core.Entities.Department> GetDepartmentByCodeAsync(string UCode);
        //  --------------------

        //  -------------------------
        //  OfficeUser
        //  -------------------------
        Task<IReadOnlyList<OfficeUser>> GetOfficeUsersAsync(AppUser appUser);
        Task<IReadOnlyList<OfficeUser>> GetOfficeUsers(ISpecification<OfficeUser> spec);
        Task<OfficeUser> GetOfficeUser(AppUser appUser);
        Task<OfficeUser> GetOfficeUserById(int Id);
        Task<OfficeUser> GetOfficeUserByEmail(string WorkEmail);
        Task<OfficeUser> GetOfficeUserByLoginId(string LoginId);
        Task<OfficeUser> GetOfficeUserByCode(string OfficeUserCode);

        Task<OfficeUser> ValidateOfficeUserAsync(OfficeUser ret, AppUser au);
        Task<IImportExcelData<OfficeUser>> BulkValidateOfficeUserAsync(IImportExcelData<OfficeUser> ret, AppUser au);
        Task<OfficeUser> SaveOfficeUser(OfficeUser officeUser, AppUser au);
        Task<bool> SaveUploadOfficeUser(IReadOnlyList<OfficeUser> officeUsers, AppUser au);
        //---Patient-------------------
        Task<IReadOnlyList<patient>> GetPatientsAsync(AppUser appUser);
        Task<patient> GetPatientByRegNoAsync(string regNo);
        Task<patient> GetPatientByCodeAsync(string Ucode);
        Task<patient> GetPatientByPatientId(int patient_id);
        Task<AppUser> GetdoctorBydoctorId(int doctor_id);
        Task<patient> ValidatePatientAsync(patient ret, AppUser au);
        Task<IReadOnlyList<patient>> GetPatientByhistoryAsync(AppUser appUser);
        Task<patient> SavePatientAsync(patient ret);
        Task<complaints> SaveComplaintsAsync(complaints ret, AppUser cu);
        Task<physicalexam> SavePhysicalexamAsync(physicalexam ret, AppUser appUser);
        Task<physicalgen> SavePhysicalgeneralAsync(physicalgen ret, AppUser cu);
        Task<pasthistory> SavePastHistoryAsync(pasthistory ret, AppUser cu);
        Task<medications> SaveMedicationsAsync(medications ret, AppUser cu);
        Task<additionalreports> SaveAdditionalReportsAsync(additionalreports ret, AppUser cu);
        Task<family> SaveFamilyHistoryAsync(family ret, AppUser cu);
        Task<systemeticexam> SavesystemicExamAsync(systemeticexam ret, AppUser cu);
        Task<List<investigations>> SaveInvestigationsAsync(List<investigations> ret,AppUser cu);
        Task<patient> UpdatePatientHistoryAsync(patient ret,AppUser cu);
        Task<IImportExcelData<patient>> BulkValidatePatientAsync(IImportExcelData<patient> ret, AppUser au);
        Task<bool> SaveUploadPatientAsync(IReadOnlyList<patient> patients, AppUser au);
        Task<appointments> SaveAppointmentAsync(appointments ret);
        Task<List<prescription>> SavePrescriptionAsync(List<prescription> ret);
        Task<List<prescription>> ValidatePrescriptionAsync(List<prescription> ret, AppUser cu);
        Task<appointments> UpdateRetrieverAppointmentAsync(appointments ret, AppUser cu);
        Task<appointments> UpdateAppointmentStatusAndDetailsAsync(appointments ret, AppUser cu);
        Task<appointmentMilestone> SaveAppointmentMilestoneAsync(appointmentMilestone ret);
        Task<appointmentMilestone> ValidateAppointmentMilestoneAsync(appointmentMilestone ret, AppUser au);
        Task<IReadOnlyList<appointments>> GetAppointmentsAsync(AppUser appUser);
        Task<IReadOnlyList<Country>> GetCountriesAsync();
        Task<IReadOnlyList<Medicine>> GetMedicinesAsync();
        Task<IReadOnlyList<potency>> GetPotenciesAsync();
        Task<IReadOnlyList<dosage>> GetDosagesAsync();
        Task<IReadOnlyList<Rate>> GetRatesAsync();
        Task<IReadOnlyList<Rate>> GetSOSAsync();
        Task<IReadOnlyList<appointments>> GetAppointmentsForRetrieversAsync(AppUser appUser);
        Task<IReadOnlyList<appointments>> GetAppointmentByPatientIdAsync(int patient_id);
        Task<appointments> ValidateAppointmentAsync(appointments ret, AppUser au);
        Task<IImportExcelData<appointments>> BulkValidateAppointmentAsync(IImportExcelData<appointments> ret, AppUser au);
        Task<SessionSetup> SaveSessionAsync(SessionSetup ret);
        Task<IReadOnlyList<SessionDoctors>> GetActiveSessionDoctorsAsync();
        Task<SessionSetup> ValidateSessionAsync(SessionSetup ret, AppUser au);
        Task<IReadOnlyList<SessionSetup>> GetSessionsAsync();
        Task<SessionSetup> GetActiveSessionAsync();
        Task<IReadOnlyList<SessionDoctors>> GetSessionDoctorsAsync(int id);
        Task<IEnumerable<SessionDoctors>> ValidateSessionDoctorsAsync(IEnumerable<SessionDoctors> ret, AppUser au);
        Task<IEnumerable<SessionDoctors>> SaveSessionDoctorsAsync(IEnumerable<SessionDoctors> doctors);
        Task<IReadOnlyList<SessionDispenseTeam>> GetSessionDispenseTeamAsync(int id);
        Task<IEnumerable<SessionDispenseTeam>> ValidateSessionDispenseTeamAsync(IEnumerable<SessionDispenseTeam> ret, AppUser au);
        Task<IEnumerable<SessionDispenseTeam>> SaveSessionDispenseTeamAsync(IEnumerable<SessionDispenseTeam> doctors);
        Task<IEnumerable<AppUser>> GetDoctorsAsync();
        Task<IEnumerable<AppUser>> GetDispenseTeamAsync();
        Task<IEnumerable<appointments>> GetAppointmentsForDoctor(int userId, string approle);
        Task<bool> SaveImeiNo(OfficeUser oe);
        Task<int> IncrementFailedCountAsync(OfficeUser ou);
       // Task SetResetFailedCount(string username, OfficeUser? ou);
    }
}
