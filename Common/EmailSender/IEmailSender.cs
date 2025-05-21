namespace EmployeeHub.Common.EmailSender
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(string toEmail, string subject, string body);
    }
}