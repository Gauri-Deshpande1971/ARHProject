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
        public string TypeOfCharges { get; set; }
        public decimal Charges { get; set; }

        public string OrganizationNameInclude { get; set; }
        public string OrganizationNameExclude { get; set; }
    }
}
