using LibraryManagement.Application.Models.Email;

namespace LibraryManagement.Application.Contracts.Email
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(EmailMessage email);
    }
}
