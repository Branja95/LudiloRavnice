namespace RentApp.Services
{
    public interface ISMTPService
    {
        void SendMail(string subject, string body, string emailTo);
    }
}
