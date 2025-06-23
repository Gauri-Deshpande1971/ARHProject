namespace API.Dtos
{
    public class physicalexamDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string T { get; set; }
        public string P { get; set; }
        public string RR { get; set; }
        public string BP { get; set; }
        public string Glands { get; set; }
        public string ENTSkin { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
    }
}
