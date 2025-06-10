namespace Core.Entities
{
    public class Attachment : BaseEntity
    {
        public string EntityName { get; set; }      //  User
        public string EntityFieldName { get; set; } //  UserID
        public string EntityKeyValue { get; set; }  //  E00001
        public string FileType { get; set; }  //  AADHAAR, PAN, etc.
        public string Filename { get; set; }
        public string PhyFilename { get; set; }
        public string UploadedByName { get; set; }
        public string UploadedOn { get; set; }
        public string UploadedIP { get; set; }

        public string UserTypeName { get; set; }
    }
}