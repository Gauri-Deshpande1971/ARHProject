using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class physicalexam:BaseEntity
    {
        public int patient_id { get; set; }
        public string T {  get; set; }
        public string P { get; set; }
        public string RR { get; set; }
        public string BP { get; set; }
        public string Glands { get; set; }
        public string ENTSkin { get; set; }
        public int height {  get; set; }    
        public int weight { get; set; } 
    }
}
