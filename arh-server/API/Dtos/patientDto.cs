using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class patientDto:BaseDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int? ADoctorId { get; set; }
        public string full_name { get; set; }
        public int age { get; set; }
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number.")]
        public string mobileNo { get; set; }
        public string? address { get; set; }
        public string? city { get; set; }
        public DateTime? DOB { get; set; }
        public string gender { get; set; }
        public string? TypeofPatient { get; set; } //(Staff/Free/Regular)
        public int? percconcession { get; set; }
        public string? RegNo { get; set; }
        public bool medicalkit { get; set; }
        [EmailAddress]
        public string? emailId { get; set; }
        public bool history { get; set; } = false;
    }
}
