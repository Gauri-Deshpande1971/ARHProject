namespace API.Dtos
{
    public class complaintsDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string? Large { get; set; }
        public string? Small { get; set; }
        public string? Medium { get; set; }
    }
}
