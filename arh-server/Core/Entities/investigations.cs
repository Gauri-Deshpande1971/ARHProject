using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class investigations:BaseEntity
    {
        public int patient_id { get; set; } 
        public string testname {  get; set; }
        public string findings { get; set; }
        public DateTime investigationdate { get; set; }
    }
}
