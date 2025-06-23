using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class complaints:BaseEntity
    {
        public int patient_id { get; set; }
        public string?  Large {  get; set; }
        public string? Small { get; set; }
        public string? Medium { get; set; }
    }
}
