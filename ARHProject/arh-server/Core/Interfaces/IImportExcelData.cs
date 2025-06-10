using System.Collections.Generic;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface IImportExcelData<T>
    {
        List<T> DataSource { get; set; }
        string ErrorMessage { get; set; }
        List<FormGridDetail> Headings { get; set; }
        List<ExcelUploadError> ErrorInFields { get; set; }
        string UploadCode { get; set; }
        void AddErrorMessage(string FieldName, string ErrorMessage, int RowNo);
        void AddErrorMessage(string ErrorMsg);

    }
}
