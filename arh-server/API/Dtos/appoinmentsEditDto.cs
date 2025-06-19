namespace API.Dtos
{
    public class appoinmentsEditDto:BaseDto
    {
        public int appointmentId { get; set; }
        public string patientName { get; set; }
        public string regNo { get; set; }
        public string category { get; set; }
        public DateTime appointmentDate { get; set; }
    }
}
