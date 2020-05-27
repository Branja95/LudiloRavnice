namespace AccountManaging.Services
{
    public interface IEmailService
    {
        void SendMail(string subject, string body, string emailTo);
    }
}
