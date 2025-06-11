using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class remedy_plans : BaseEntity
    {
        public int patient_id { get; set; }
        public string chronic_remedy { get; set; }
        public string chronic_totality { get; set; }
        public string phasic_remedy { get; set; }
        public string phasic_totality { get; set; }
        public string acute_remedy { get; set; }
        public string acute_totality { get; set; }
    }
}
