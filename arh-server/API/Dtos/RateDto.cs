namespace API.Dtos
{
    public class RateDto:BaseDto
    {
        public int Id { get; set; }
        public string Description { get; set; }// 1 Day, 2 Day, SOS-S, SOS-M etc.
        public int medicineId { get; set; }
        public string medicineName { get; set; }
        public decimal TelCharges { get; set; }
        public decimal Charges { get; set; }
    }
}