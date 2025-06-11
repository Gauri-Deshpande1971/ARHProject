using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class medicineKit:BaseEntity
    {
        public int patient_id { get; set; }
        public decimal amount { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool paid { get; set; }
        public DateTime payment_date { get; set; }
        public string notes { get; set; }   
    }
}
