using System.Collections.Generic;

namespace Core.Models
{
    public class NavmenuOfUser
    {
        public int Id { get; set; }
        public int ParId { get; set; }
        public string NavMenuName { get; set; }
        public string Description { get; set; }
        public string NavLink { get; set; }
        public string IconClass { get; set; }
        public int SrNo { get; set; }

        public List<NavmenuOfUser> Submenus {get; set;}
    }
}