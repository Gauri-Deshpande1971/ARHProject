namespace API.Dtos
{
    public class OrganizationDto : BaseDto
    {
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string CurrentStartDate { get; set; }
        public string CurrentEndDate { get; set; }

        public string ApplicationEndDate { get; set; }
        public string ApprovalEndDate { get; set; }

    }
}
