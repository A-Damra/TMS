namespace TMS.Services
{
    public interface IEmailService
    {
        Task SendEmail(string title, string email, string body);
    }
}
