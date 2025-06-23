namespace API.Dtos
{
    public class additionalreportsDto:BaseDto
    {
        public int Id { get; set; } 
        public int patient_id { get; set; }
        public string reports { get; set; }
    }
}
