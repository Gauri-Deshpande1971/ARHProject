using System;
namespace Core.Entities
{
    public class OfficeUser : BaseEntity
    {
        public required string OfficeUserName { get; set; } = "";
        public string? OfficeUserCode { get; set; }
        /// <summary>
        /// This is referring to Id from AppUser entity / table
        /// </summary>
        public required string AppUserId { get; set; } = "";
        public string? WorkEmail { get; set; }
        public string? AlternateEmail { get; set; }
        public string? MobileNo { get; set; }
        public string? AlternateMobileNos { get; set; }
        public string? IMEINo { get; set; }

        public DateTime? LastLogin { get; set; }
        public int FailedLoginCount { get; set; }

        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public required string AppRoleCode { get; set; } = "USER";
        public int AppRoleId { get; set; }

        public bool IsAdmin { get; set; }
        public string? LoginId { get; set; }

        public string? GenderName { get; set; }
        public string? Title { get; set; }       //  Mr, Ms, Dr
        public required string OrganizationName { get; set; } = "";
        public string? AppMode { get; set; }    //  WEB or MOBILE
        public string? OfficerTypeName { get; set; }            //  Staff, Technician

        public Designation? Designation { get; set; }
        public Department? Department { get; set; }
        public required AppRole AppRole { get; set; }

        public OfficeUser()
        {
            FailedLoginCount = 0;
            IsAdmin = false;
            AppMode = "WEB"; // Default to WEB
            OrganizationName = "Default Organization"; // Default value, can be changed later
        }
    }
}
