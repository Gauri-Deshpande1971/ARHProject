namespace API.Dtos
{
    public class pasthistoryDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string? history { get; set; }
    }
}
