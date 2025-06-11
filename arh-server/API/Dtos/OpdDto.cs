namespace API.Dtos
{
    public class OpdDto:BaseDto
    {
        public int Id { get; set; }
        public string OpdName { get; set; }
        public string OrganizationNameInclude { get; set; }
        public string OrganizationNameExclude { get; set; }
    }
}
