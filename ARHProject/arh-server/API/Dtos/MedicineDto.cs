namespace API.Dtos
{
    public class MedicineDto:BaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganizationNameInclude { get; set; }
        public string OrganizationNameExclude { get; set; }
    }
}
