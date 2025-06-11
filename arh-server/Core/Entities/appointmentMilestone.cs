using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class appointmentMilestone:BaseEntity
    {
        public int appointmentId { get; set; }
        public string milestone { get; set; }
        public DateTime milestoneTime { get; set; }
        public string userId { get; set; }
    }
}
