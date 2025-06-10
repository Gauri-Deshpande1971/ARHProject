namespace Core.Models
{
public class ExcelUploadError
    {
        public int rowno { get; set; }
        public string fieldname { get; set; }
        public string errormessage { get; set; }
    }
}
