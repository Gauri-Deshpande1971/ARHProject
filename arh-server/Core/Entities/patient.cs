using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Entities
{
    public class patient : BaseEntity
    {
        public int DoctorId { get; set; }
        public int? ADoctorId { get; set; }
        public string full_name { get; set; }
        public int age { get; set; }
        public string mobileNo { get; set; }
        public string? address { get; set; }
        public string? city { get; set; }
        public DateTime? DOB { get; set; }
        public string gender { get; set; }
        public string? TypeofPatient { get; set; } //(Staff/Free/Regular)
        public int? percconcession { get; set; }
        public string? RegNo { get; set; }
        public bool medicalkit { get; set; }
        public string? emailId { get; set; }
        public bool history { get; set; } = false;
    }
}
