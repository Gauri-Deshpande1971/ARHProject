using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class opd_doc:BaseEntity
    {
        public int OpdId {  get; set; }
        public int UserId { get; set; }
    }
}
