namespace API.Dtos
{
    public class appointmentMilestoneDto
    {
        public int Id { get; set; }
        public int appointmentId { get; set; }
        public string milestone { get; set; }
        public DateTime milestoneTime { get; set; }
        public string userId { get; set; }
    }
}
