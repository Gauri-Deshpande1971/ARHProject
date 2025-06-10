namespace Core.Entities
{
    public class Organization : BaseEntity
    {
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }

        public DateTime CurrentStartDate { get; set; }
        public DateTime CurrentEndDate { get; set; }

        public DateTime ApplicationEndDate { get; set; }
        public DateTime ApprovalEndDate { get; set; }
    }
}

