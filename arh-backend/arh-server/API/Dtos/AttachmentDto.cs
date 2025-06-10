namespace API.Dtos
{
    public class AttachmentDto
    {
        public string EntityName { get; set; }      //  User
        public string EntityFieldName { get; set; } //  Aadhaar
        public string EntityKeyValue { get; set; }  //  1234 1122 4321
        public string FileType { get; set; }  //  Aadhaar Card
        public string Filename { get; set; }
        public string PhyFilename { get; set; }
        public string UploadedByName { get; set; }
        public string UploadedOn { get; set; }
        public string UploadedIP { get; set; }

        public string UserTypeName { get; set; }

    }
}
