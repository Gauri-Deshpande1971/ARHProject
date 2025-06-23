namespace API.Dtos
{
    public class medicationsDto:BaseDto
    {
        public int Id { get; set; } 
        public int patient_id { get; set; }
        public string details { get; set; }
    }
}
