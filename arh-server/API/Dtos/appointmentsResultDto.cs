namespace API.Dtos
{
    public class appointmentsViewDto : BaseDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsActive { get; set; }
        public string PatientFullName { get; set; }
        public string PatientRegNo { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string status { get; set; }
        public string RowBackColor { get; set; }
        public int OfficeUserId { get; set; }
    }
}
