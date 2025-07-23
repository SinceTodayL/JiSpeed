
namespace JISpeed.Core.Configurations
{
    public class SmtpSettings
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FromEmail { get; set; }
        public required string DisplayName { get; set; }
        public required bool EnableSsl { get; set; }
        public required string FrontendBaseUrl { get; set; }
        
    }
}