using System.Net.Mail;

namespace RentApp.Services
{
    public class SMPTService : ISMTPService
    {
        public void SendMail(string subject, string body, string emailTo)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.To.Add(emailTo);
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new MailAddress("branici@gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("branici@gmail.com", "brankojelic");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }
    }
}