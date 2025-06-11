namespace API.Dtos
{
    public class medicalkitDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public decimal amount { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool paid { get; set; }
        public DateTime payment_date { get; set; }
        public string notes { get; set; }
    }
}
