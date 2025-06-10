using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Infrastructure.Services
{
    public class EmailerService
    {

        public String SendEmail(string gSMTPHost, int gSMTPPort, Boolean gSMTPAuthentication,
            string gSMTPUser, string gUserDisplayName, string gSMTPPass, string gSMTPDomain, Boolean gSMTPSSL,
            string sendto, string sendToBCC, string sendToCC, Boolean IsSendSeparate,
            string Subject, string Matter, Boolean IsBodyHtml,
            string Attachements)
        {

            SmtpClient tmpClient = null;
            System.Net.CredentialCache tmpCredentials = null;
            MailMessage tmpMessage;
            MailAddress tmpMailAddress;

            try
            {
                tmpClient = new SmtpClient(gSMTPHost, gSMTPPort);
                tmpClient.UseDefaultCredentials = false;

                if (gSMTPAuthentication)
                {
                    tmpCredentials = new System.Net.CredentialCache();

                    if (gSMTPSSL)
                    {
                        if (tmpCredentials.GetCredential(gSMTPHost, gSMTPPort, "SSL") == null)
                            tmpCredentials.Add(gSMTPHost, gSMTPPort, "SSL", new NetworkCredential(gSMTPUser, gSMTPPass));

                        tmpClient.Credentials = tmpCredentials.GetCredential(gSMTPHost, gSMTPPort, "SSL");
                    }
                    else
                    {
                        if (tmpCredentials.GetCredential(gSMTPHost, gSMTPPort, "Basic") == null)
                            tmpCredentials.Add(gSMTPHost, gSMTPPort, "Basic", new NetworkCredential(gSMTPUser, gSMTPPass));
                        tmpClient.Credentials = tmpCredentials.GetCredential(gSMTPHost, gSMTPPort, "Basic");
                    }
                }

                if (!string.IsNullOrEmpty(gSMTPPass))
                {
                    tmpClient.Credentials = new NetworkCredential(gSMTPUser, gSMTPPass);    //  gSMTPDomain
                }
                else
                {
                    tmpClient.Credentials = new NetworkCredential();
                }

                tmpMailAddress = new MailAddress(gSMTPUser, gUserDisplayName);

                if (IsSendSeparate)
                {
                    foreach (String tmp__SendTo in sendto.Split(','))
                    {
                        if (!String.IsNullOrEmpty(tmp__SendTo))
                            tmpMessage = new MailMessage(tmpMailAddress, new MailAddress(tmp__SendTo.Trim().Replace(",", "")));
                        else
                        {
                            tmpMessage = new MailMessage();
                            tmpMessage.From = tmpMailAddress;
                        }

                        if (!String.IsNullOrEmpty(sendToBCC))
                        {
                            foreach (String tmp__SendToBCC in sendToBCC.Split(','))
                            {
                                if (!String.IsNullOrEmpty(tmp__SendToBCC))
                                {
                                    tmpMessage.Bcc.Add(new MailAddress(tmp__SendToBCC.Replace(",", "")));
                                }
                            }
                        }
                        if (!String.IsNullOrEmpty(sendToCC))
                        {
                            foreach (String tmp__SendToCC in sendToCC.Split(','))
                            {
                                if (!String.IsNullOrEmpty(tmp__SendToCC))
                                {
                                    tmpMessage.CC.Add(new MailAddress(tmp__SendToCC.Replace(",", "")));
                                }
                            }
                        }

                        tmpMessage.Subject = Subject;
                        tmpMessage.Body = Matter;
                        tmpMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        tmpMessage.IsBodyHtml = IsBodyHtml;
                        tmpClient.EnableSsl = gSMTPSSL;

                        //   Attachments if any
                        if (!String.IsNullOrEmpty(Attachements))
                        {
                            foreach (string tmpAttach in Attachements.Split(','))
                            {
                                tmpMessage.Attachments.Add(new System.Net.Mail.Attachment(tmpAttach));
                            }
                        }

                        tmpClient.Timeout = 60000;
                        tmpClient.Send(tmpMessage);
                    }
                }
                else
                {
                    tmpMessage = new MailMessage();

                    tmpMessage.From = tmpMailAddress;

                    if (!String.IsNullOrEmpty(sendto))
                    {
                        foreach (String tmp__SendTo in sendto.Split(','))
                        {
                            if (!String.IsNullOrEmpty(tmp__SendTo))
                                tmpMessage.To.Add(new MailAddress(tmp__SendTo.Trim().Replace(",", "")));
                            //else
                            //{
                            //    tmpMessage = new MailMessage();
                            //    tmpMessage.From = tmpMailAddress;
                            //}
                        }
                    }
                    if (!String.IsNullOrEmpty(sendToBCC))
                    {
                        foreach (String tmp__SendToBCC in sendToBCC.Split(','))
                        {
                            if (!String.IsNullOrEmpty(tmp__SendToBCC))
                            {
                                tmpMessage.Bcc.Add(new MailAddress(tmp__SendToBCC.Replace(",", "")));
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(sendToCC))
                    {
                        foreach (String tmp__SendToCC in sendToCC.Split(','))
                        {
                            if (!String.IsNullOrEmpty(tmp__SendToCC))
                            {
                                tmpMessage.CC.Add(new MailAddress(tmp__SendToCC.Replace(",", "")));
                            }
                        }
                    }

                    tmpMessage.Subject = Subject;
                    tmpMessage.Body = Matter;
                    tmpMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    tmpMessage.IsBodyHtml = IsBodyHtml;

                    tmpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    tmpClient.EnableSsl = gSMTPSSL;

                    //   Attachments if any
                    if (!String.IsNullOrEmpty(Attachements))
                    {
                        foreach (string tmpAttach in Attachements.Split(','))
                        {
                            tmpMessage.Attachments.Add(new System.Net.Mail.Attachment(tmpAttach));
                        }
                    }
                    tmpClient.Timeout = 60000;  //  1 minute
                    tmpClient.Send(tmpMessage);
                }

            }
            catch (Exception ex)
            {
                //throw ex;
                return "Error: " + ex.Message;
            }

            return "Success";
        }
    
        
    }
}
