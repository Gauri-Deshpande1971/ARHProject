namespace API.Dtos
{
    public class familyDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string familyhistory { get; set; }
        public string familysetup { get; set; }
    }
}
