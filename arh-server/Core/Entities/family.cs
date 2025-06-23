using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class family:BaseEntity
    {
        public int patient_id { get; set; }
        public string familyhistory {  get; set; }
        public string familysetup { get; set; }
    }
}
