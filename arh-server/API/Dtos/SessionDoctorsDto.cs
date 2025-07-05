namespace API.Dtos
{
    public class SessionDoctorsDto:BaseDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int DoctorId { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public Guid UCode { get; set; }
    }
}
