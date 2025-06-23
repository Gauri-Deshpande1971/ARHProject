namespace API.Dtos
{
    public class investigationsDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string testname { get; set; }
        public string findings { get; set; }
        public DateTime investigationdate { get; set; }
    }
}
