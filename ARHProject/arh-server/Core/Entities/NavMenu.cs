namespace Core.Entities
{
    public class NavMenu : BaseEntity
    {
        public int ParId { get; set; }
        public string NavMenuName { get; set; }
        public string Description { get; set; }
        public string NavLink { get; set; }
        public string IconClass { get; set; }
        public int SrNo { get; set; }
        public string AppRoleCode { get; set; }
        public string AppMode { get; set; } = "WEB,MOBILE,";
    }
}
