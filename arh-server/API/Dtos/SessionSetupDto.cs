namespace API.Dtos
{
    public class SessionSetupDto:BaseDto
    {
        public int Id { get; set; }
        public string SessionName { get; set; }
        public DateTime SessionDate { get; set; }
    }
}
