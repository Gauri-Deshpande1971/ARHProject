using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SessionSetup:BaseEntity
    {
        public string SessionName { get; set; }
        public DateTime SessionDate { get; set; }
    }
}
