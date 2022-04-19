namespace UserServices.Services.MailService
{
  public class MailConfig
  {
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPassword { get; set; }
    public string From { get; set; }
    public string ReplyTo { get; set; }
  }
}