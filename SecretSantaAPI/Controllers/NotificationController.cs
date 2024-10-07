using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Business;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly EmailSender _emailSender;

    // Inject EmailSender via constructor
    public NotificationController(EmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    // API endpoint to send an email
    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromQuery] string recipientEmail, [FromQuery] string subject, [FromQuery] string message)
    {
        // Call the email sender method
        await _emailSender.SendEmailAsync(recipientEmail, subject, message);

        return Ok("Email sent successfully!");
    }
    public class EmailRequest
    {
        public required string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public required string MessageBody { get; set; }
    }
}
