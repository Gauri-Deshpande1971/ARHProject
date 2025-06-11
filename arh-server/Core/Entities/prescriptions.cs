using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class prescription:BaseEntity
    {
      public int appointment_id { get; set; }
      public int medicineId { get; set; }
      public int potencyId { get; set; }
      public int dosageId { get; set; }
      public int rateId { get; set; }//noofdays
      public int sosId { get; set; } // medicine not mandatory
      public string notes { get; set; }
    }
}
