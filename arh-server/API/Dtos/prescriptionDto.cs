namespace API.Dtos
{
    public class prescriptionDto:BaseDto
    {
        public int Id { get; set; }
        public int appointment_id { get; set; }
        public int medicineId { get; set; }
        public int potencyId { get; set; }
        public int dosageId { get; set; }
        public int rateId { get; set; }//noofdays
        public int sosId { get; set; } // medicine not mandatory default 0
        public string notes { get; set; }

    }
}
