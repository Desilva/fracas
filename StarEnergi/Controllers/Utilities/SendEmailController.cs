using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Configuration;
using System.Diagnostics;
using StarEnergi.Models;

namespace StarEnergi.Controllers.Utilities
{
    public class SendEmailController : Controller
    {
        //
        // GET: /SendEmail/
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        
        public ActionResult Index()
        {
            return Content("Index");

        }

        private string EmailsToString(List<string> email)
        {
            string result = "";
            for (int i = 0; i < email.Count(); i++)
            {
                if (i != email.Count() - 1)
                {
                    result += email[i] + ";";
                }
                else
                {
                    result += email[i];
                }

            }
            return result;
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
                smtp.UseDefaultCredentials = false;
                System.Net.NetworkCredential nc = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpuser"], ConfigurationManager.AppSettings["smtppassword"]);
                smtp.Credentials = (System.Net.ICredentialsByHost)nc.GetCredential(ConfigurationManager.AppSettings["smtp"], 25, "Basic");
                smtp.Port = Int32.Parse(ConfigurationManager.AppSettings["smtpport"]);                
                smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["ssl"]);

                try
                {
                    smtp.Send(email);
                }
                catch (SmtpException e)
                {
                    //save email to database and send the others
                    email_error te = new email_error();
                    te.emails = EmailsToString(to);
                    te.content = message;
                    te.subject = subject;
                    te.exception = e.Message + "#" + e.StackTrace;
                    db.email_error.Add(te);
                    db.SaveChanges();
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
