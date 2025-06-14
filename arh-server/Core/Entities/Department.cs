namespace Core.Entities
{
    public class Department : BaseEntity
    {
        public int ParId { get; set; }      //  if 0 indicates Department, else Subdepartment
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }

        public string OrganizationNameInclude { get; set; }
        public string OrganizationNameExclude { get; set; }
    }
}
