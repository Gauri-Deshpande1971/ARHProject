using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class NavmenuOfSuperUser
    {
        public int Id { get; set; }
        public int ParId { get; set; }
        public required string NavMenuName { get; set; } = "";
        public string? Description { get; set; }
        public string? NavLink { get; set; }
        public string? IconClass { get; set; }
        public int SrNo { get; set; }
        public required string AppRoleCode { get; set; } = "";
        public required string UCode { get; set; } = "";
        public bool Selected { get; set; }
        public List<NavmenuOfSuperUser>? Submenus { get; set; }
        public NavmenuOfSuperUser()
        {
            Submenus = new List<NavmenuOfSuperUser>();
        }
        public NavmenuOfSuperUser(int id, int parId, string navMenuName, string? navLink, string? iconClass, int srNo, string appRoleCode, string uCode)
        {
            Id = id;
            ParId = parId;
            NavMenuName = navMenuName;
            NavLink = navLink;
            IconClass = iconClass;
            SrNo = srNo;
            AppRoleCode = appRoleCode;
            UCode = uCode;
            Submenus = new List<NavmenuOfSuperUser>();
        }
    }
}
