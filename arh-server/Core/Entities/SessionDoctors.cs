using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SessionDoctors:BaseEntity
    {
        public int SessionId {  get; set; } 
        public int DoctorId { get; set; }   
    }
}
