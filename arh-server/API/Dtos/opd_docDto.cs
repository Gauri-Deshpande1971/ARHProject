namespace API.Dtos
{
    public class opd_docDto
    {
        public int Id { get; set; }
        public int OpdId { get; set; }
        public int UserId { get; set; }
        public string OrganizationNameInclude { get; set; }
        public string OrganizationNameExclude { get; set; }
    }
}
