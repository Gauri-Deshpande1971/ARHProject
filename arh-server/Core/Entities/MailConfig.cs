namespace Core.Entities
{
    public class MailConfig : BaseEntity
    {
        public string MailAction { get; set; }
        public string MailDescription { get; set; }
        public string MailFromName { get; set; }
        public string MailFromEmail { get; set; }
        public string MailFromPwd { get; set; }
        public string MailBcc { get; set; }
        public string MailCc { get; set; }
        public string MailSubject { get; set; }
        public string MailContent { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool IsAuthenticationReq { get; set; }
        public bool IsSslReq { get; set; }
        public bool IsHtml { get; set; }

        public string OrganizationNameInclude { get; set; }

    }
}
