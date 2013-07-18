using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Configuration;
using System.Diagnostics;

namespace StarEnergi.Controllers.Utilities
{
    public class SendEmailController : Controller
    {
        //
        // GET: /SendEmail/

        public ActionResult Index()
        {
            return Content("Index");

        }

        public ActionResult Send(List<String> to, string message, string subject) {
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
            foreach (string t in to)
            {
                if(t != null){
                    email.To.Add(t); //recipient
                }
            }
            if (email.To.Count > 0)
            {
                email.Subject = subject;
                email.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["smtpuser"]); //from email
                email.IsBodyHtml = true;
                email.Body = message;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["smtp"]);  // you need an smtp server address to send emails
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpuser"], ConfigurationManager.AppSettings["smtppassword"]);
                smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["smtpport"]);
                smtp.EnableSsl = true;
                try
                {
                    smtp.Send(email);
                }
                catch (SmtpException e)
                {
                    
                }
                finally
                {
                    smtp.Dispose();
                }
            }
            return Content("Email Sent!");
        }

    }
}
