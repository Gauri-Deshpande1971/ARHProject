using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class physicalgen:BaseEntity
    {
        public int patient_id { get; set; }
        public string? appetite {  get; set; }
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
