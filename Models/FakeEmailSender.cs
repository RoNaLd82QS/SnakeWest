using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace VentaZapatillas.Models
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // No enviamos correos realmente.
            return Task.CompletedTask;
        }
    }
}