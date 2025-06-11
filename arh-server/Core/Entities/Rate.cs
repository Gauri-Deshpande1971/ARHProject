using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Rate:BaseEntity
    {       
        public string Description { get; set; }
        public int medicineId { get; set; }
        public string medicineName { get; set; }
        public decimal TelCharges { get; set; }
        public decimal Charges { get; set; }
    }
}
