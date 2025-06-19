namespace API.Dtos
{
    public class SessionDoctorsDto:BaseDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int DoctorId { get; set; }
    }
}
