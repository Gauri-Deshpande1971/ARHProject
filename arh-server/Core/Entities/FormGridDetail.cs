namespace Core.Entities
{
    public class FormGridDetail : BaseEntity
    {
        public int FormGridHeaderId { get; set; }
        public  FormGridHeader FormGridHeader { get; set; }
        public required string FieldName { get; set; }
        public required string FieldHeading { get; set; }
        public bool IsSortable { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsResizable { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEditable { get; set; }
        public bool IsPinned { get; set; }
        public int Position { get; set; }
        public int Width { get; set; }
        public required string FieldType { get; set; }
        public string? FieldFormat { get; set; }
        public string? HeaderBackColor { get; set; }
        public string? HeaderForeColor { get; set; }
        public string? ColumnBackColor { get; set; }
        public string? ColumnForeColor { get; set; }
        public string? RowBackColor { get; set; }
        public string? RowForeColor { get; set; }
        public string? RowHighlightedColor { get; set; }
        public string? SelectedRowHighlightedColor { get; set; }
        public bool CanExportExcel { get; set; }
        public bool CanExportCsv { get; set; }
        public bool CanImportExcel { get; set; }
        public bool CanImportCsv { get; set; }
        public bool IsCompulsoryForImport { get; set; }
    }
}