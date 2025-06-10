namespace Core.Entities
{
    public class FormGridHeader : BaseEntity
    {
        public string FormName { get; set; }
        public string TableName { get; set; }
        public bool IsPagination { get; set; }
        public int NoOfRecords { get; set; }
        public bool CanExportExcel { get; set; }
        public bool CanExportCsv { get; set; }
        public bool CanUserCustomize { get; set; }
        public bool CanImportExcel { get; set; }
        public bool CanImportCsv { get; set; }
        public int OfficeUserId { get; set; }
        public string AppRoleCode { get; set; }
    }
}