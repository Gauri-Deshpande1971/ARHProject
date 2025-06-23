using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class medications:BaseEntity
    {
        public int patient_id { get; set; }
        public string details { get; set; }
    }
}
