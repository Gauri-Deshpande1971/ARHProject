namespace API.Dtos
{
    public class sessionDetailsDto:BaseDto
    {
       public int SessionId     { get; set; }
        public string SessionName { get; set; }
        public DateTime SessionDate { get; set; }
        public bool IsActive { get; set; }
        public List<SessionDoctorsDto> SessionDoctors { get; set; }
        public List<SessionDispenseTeamDto> SessionDispenses { get; set; }
    }
}
