namespace API.Dtos
{
    public class physicalgenDto:BaseDto
    {
        public int Id { get; set; }
        public int patient_id { get; set; }
        public string? appetite { get; set; }
        public string? thirst { get; set; }
        public string? cravings { get; set; }
        public string? aversions { get; set; }
        public string? stools { get; set; }
        public string? urine { get; set; }
        public string? perspiration { get; set; }
        public string? sleepdream { get; set; }
        public string? menstrualobs { get; set; }
        public string? themalreaction { get; set; }
    }
}
