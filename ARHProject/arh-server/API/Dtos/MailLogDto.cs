using System;

namespace API.Dtos
{
    public class MailLogDto : BaseDto
    {
        public string MailQueuedOn { get; set; }
        public string MailStatus { get; set; }
        public int MailConfigId { get; set; }
        public string MailFromName { get; set; }
        public string MailFromEmail { get; set; }
        public string MailFromPwd { get; set; }
        public string MailTo { get; set; }
        public string MailBcc { get; set; }
        public string MailCc { get; set; }
        public string MailSubject { get; set; }
        public string MailContent { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool IsAuthenticationReq { get; set; }
        public bool IsSslReq { get; set; }
        public bool IsHtml { get; set; }

        public string MailError { get; set; }
        public string EntityTypeName { get; set; }
        public int? EntityId { get; set; }
        public string SecondaryEntityTypeName { get; set; }
        public int? SecondaryEntityId { get; set; }

    }
}
