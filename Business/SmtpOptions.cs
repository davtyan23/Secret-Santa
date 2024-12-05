namespace SecretSantaAPI;

public class SmtpOptions
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPassword { get; set; }
    public bool EnableSsl { get; set; }
}
