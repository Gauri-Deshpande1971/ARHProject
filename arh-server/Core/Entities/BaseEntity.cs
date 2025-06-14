using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Interfaces;
using Core.Models;

namespace Core.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public System.Boolean IsDeleted { get; set; }
        public System.Boolean IsActive { get; set; }

        public Guid UCode { get; set; }
        public int SequenceNo { get; set; }
        public int? ExtraId1 { get; set; }
        public int? ExtraId2 { get; set; }

        public string? ExtraValue1 { get; set; }
        public string? ExtraValue2 { get; set; }

        public string? LogHistory { get; set; }

        public string? JsonData { get; set; }
        // public DateTime? EditedOn { get; set; }
        // public string EditedCode { get; set; }
        // public string EditByUser { get; set; }

        [NotMapped]
        public ExcelUploadError Errors { get; set; }

        public BaseEntity()
        {
            CreatedOn = DateTime.UtcNow;
            IsDeleted = false;      //  Not deleted
            IsActive = true;
            UCode = Guid.NewGuid();
        }

        public void CopyBeforeUpdate(BaseEntity src, string ChangedByUsername)
        {
            ExtraId1 = src.ExtraId1;
            ExtraId2 = src.ExtraId2;
            ExtraValue1 = src.ExtraValue1;
            ExtraValue2 = src.ExtraValue2;
            LogHistory = src.LogHistory;

            string lx = (string.IsNullOrEmpty(src.LogHistory)) ? "" : src.LogHistory;
            lx = "{ \"ChangedByUsername\" : \"" + ChangedByUsername + "\"" + 
                    ", \"ChangedOn\" : \"" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.UtcNow) + "\"}, " +
                    " \"Object\" : " + System.Text.Json.JsonSerializer.Serialize(src) + " } \r\n" + lx;
            LogHistory = lx;
        }

        public void AddErrorMessage(string ErrorMessage)
        {
            if (Errors == null) 
            {
                Errors = new ExcelUploadError();
            }
            if (string.IsNullOrEmpty(Errors.errormessage))
                Errors.errormessage = "";

            Errors.errormessage += ErrorMessage + "\r\n";
        }

        public bool HasErrors()
        {
            if (Errors == null)
                return false;
        
            if (string.IsNullOrEmpty(Errors.errormessage))
            {
                return false;
            }

            return true;
        }

        public string GetErrors()
        {
            if (Errors == null)
                return "";
        
            if (string.IsNullOrEmpty(Errors.errormessage))
            {
                return "";
            }

            return Errors.errormessage;
        }
    }
}
