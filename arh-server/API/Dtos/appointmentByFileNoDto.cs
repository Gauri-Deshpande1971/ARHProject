namespace API.Dtos
{
    public class appointmentByFileNoDto:BaseDto
    {
        public int patient_id { get; set; }
        public DateTime visit_date { get; set; }
        public string category { get; set; }//FU,PC,CR
        public string status { get; set; }//A
    }
}
