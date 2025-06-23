using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class pasthistory :BaseEntity
    {
        public int patient_id { get; set; }
        public string? history {  get; set; }    
    }
}
