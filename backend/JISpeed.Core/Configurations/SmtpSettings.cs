
namespace JISpeed.Core.Configurations
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string DisplayName { get; set; }
        public bool EnableSsl { get; set; }
        public string FrontendBaseUrl { get; set; }
    }
}