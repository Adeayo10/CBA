using CBA.Models;

namespace CBA.Services;
public interface IEmailService
{
    Task SendEmail(Message message);
}