namespace API.Dtos
{
    public class patientDto:BaseDto
    {
        public int Id { get; set; }
        public string full_name { get; set; }
        public int age { get; set; }
        public DateTime DOB { get; set; }
        public string gender { get; set; }
        public string TypeofPatient { get; set; } //(Staff/Free/Regular)
        public int percconcession { get; set; }
        public string RegNo { get; set; }
        public bool medicalkit { get; set; }
        public string emailId { get; set; }
    }
}
