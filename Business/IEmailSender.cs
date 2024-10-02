using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string recipientEmail, string subject, string messageBody);
    }
}
