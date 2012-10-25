using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Collections.Specialized;
using System.Net.Mail;


namespace cardeira.p2s
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void ProcessRequest(HttpContext context)
        {            
            logger.Debug("process request " + context.Request.Form);
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartAsyncTask), context);
        }

        public static void PRequest(HttpContext context)
        {
           
            new Handler1().ProcessRequest(context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void StartAsyncTask(Object workItemState)
        {
            try
            {
                NameValueCollection nc = ((HttpContext)workItemState).Request.Form;
                new GDataAPI(nc);
                SendEMail(nc);
            }
            catch (Exception e)
            {
                logger.FatalException("exception on asynctask", e);
            }
        }

        private void SendEMail(NameValueCollection nc)
        {            
            try
            {
                string body = cardeira.Properties.Settings.Default.emailSubject +"\n";
                foreach (string key in nc)
                    body += (key + ": " + nc[key] + "\n");

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("email-smtp.us-east-1.amazonaws.com");

                mail.From = new MailAddress("joaquim@cardeira.com");
                string[] emailsTO = cardeira.Properties.Settings.Default.emailTO.Split(',');
                foreach(string email in emailsTO)
                    mail.To.Add(email);
                mail.Subject = cardeira.Properties.Settings.Default.emailSubject;
                mail.Body = body;

                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential("AKIAJNQBS6QLQUJWFLXA", "AtYmfR62I7TxDcLehX7MUI2sCgkfez/2iS4jgSP5kk9T");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                logger.ErrorException("error sending e-mail", ex);
            }
        }
    }
}