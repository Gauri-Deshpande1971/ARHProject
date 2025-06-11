using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Core.Entities
{
    public class appointments:BaseEntity
    {
     public int patient_id {  get; set; }				
     public DateTime visit_date { get; set; }
     public string status { get; set; }// (A/D/AD/DIS/CAS/DISP)				
     public int assistantDoctorId { get; set; }//if assistant doctor does the followUp
     public decimal medicinecharges {  get; set; }			
     public decimal courierCharges { get; set; }// (Only for courier patients)				
     public decimal consultationcharges {  get; set; }
     public string Casepaper { get; set; }  
     public DateTime consultationchargesPaidOn {  get; set; }
     public string ModeofPayment {  get; set; }
    }
}
