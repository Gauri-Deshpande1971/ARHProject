namespace API.Dtos
{
    public class clinical_diagnosisDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string regNo { get; set; }
        public string diagnosis_name { get; set; }
        public string system_pathology { get; set; }
        public string follow_up_criteria { get; set; }
        public string review_1 { get; set; }
        public string review_2 { get; set; }
        public string review_3 { get; set; }
        public string com_review_1 { get; set; }
        public string com_review_2 { get; set; }
        public string com_review_3 { get; set; }
    }
}
