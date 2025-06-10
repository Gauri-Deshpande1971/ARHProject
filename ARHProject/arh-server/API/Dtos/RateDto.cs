namespace API.Dtos
{
    public class RateDto:BaseDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string TypeOfCharges { get; set; }
        public decimal Charges { get; set; }
        public string OrganizationNameInclude { get; set; }
        public string OrganizationNameExclude { get; set; }
    }
}