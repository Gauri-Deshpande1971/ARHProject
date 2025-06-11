namespace API.Dtos
{
    public class remedy_plansDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string chronic_remedy { get; set; }
        public string chronic_totality { get; set; }
        public string phasic_remedy { get; set; }
        public string phasic_totality { get; set; }
        public string acute_remedy { get; set; }
        public string acute_totality { get; set; }
    }
}
