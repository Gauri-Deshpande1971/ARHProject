using System.Collections.Generic;
using Core.Interfaces;
using Core.Entities;
using ClosedXML.Excel;
using System;
using System.Linq;
using System.Reflection;
using Core.Models;
using Infrastructure.Services;

namespace Infrastructure.Utility
{
    public class ImportExcelData<T> : IImportExcelData<T>
    {
        public ImportExcelData()
        {
            DataSource = new List<T>();
            Headings = new List<FormGridDetail>();
            ErrorInFields = new List<ExcelUploadError>();
            ErrorMessage = "";
            UploadCode = "";
        }
        public List<T> DataSource { get; set ; }
        public string ErrorMessage  { get; set ; }
        public List<FormGridDetail> Headings { get; set ; }
        public List<ExcelUploadError> ErrorInFields { get; set ; }
        public string UploadCode { get; set; }
        public void AddErrorMessage(string FieldName, string ErrorMessage, int RowNo)
        {
            if (ErrorInFields == null)
            {
                ErrorInFields = new List<ExcelUploadError>();
            }
            if (ErrorInFields.Where(x => x.fieldname == FieldName && x.rowno == RowNo).Any())
            {
                ErrorInFields.Where(x => x.fieldname == FieldName && x.rowno == RowNo)
                    .FirstOrDefault()
                    .errormessage += ", " + ErrorMessage;
            }
            else
            {
                ErrorInFields.Add(new ExcelUploadError 
                {
                    fieldname = FieldName, errormessage = ErrorMessage, rowno = RowNo
                });
            }
        }

        public void AddErrorMessage(string ErrorMsg)
        {
            if (string.IsNullOrEmpty(this.ErrorMessage))
                this.ErrorMessage = ErrorMsg;
            else
                this.ErrorMessage += "\r\n" + ErrorMsg;
        }
    }
}