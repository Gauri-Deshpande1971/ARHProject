using System;

namespace Core.Entities
{
    public class MailLog : BaseEntity
    {
        public DateTime MailQueuedOn { get; set; }
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


        public void CopyFromConfig(MailConfig mc, string content, string sendto)
        {
            CreatedOn = DateTime.UtcNow;
            IsActive = true;
            IsAuthenticationReq = mc.IsAuthenticationReq;
            IsDeleted = false;
            IsHtml = mc.IsHtml;
            IsSslReq = mc.IsSslReq;
            MailBcc = mc.MailBcc;
            MailCc = mc.MailCc;
            MailConfigId = mc.Id;
            MailContent = content;
            MailFromEmail = mc.MailFromEmail;
            MailFromName = mc.MailFromName;
            MailFromPwd = mc.MailFromPwd;
            MailQueuedOn = DateTime.UtcNow;
            MailStatus = "Queued";
            MailSubject = mc.MailSubject;
            SmtpPort = mc.SmtpPort;
            SmtpServer = mc.SmtpServer;
            MailTo = sendto;
        }
    }
}
