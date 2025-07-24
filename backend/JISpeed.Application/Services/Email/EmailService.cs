using System.Globalization;
using JISpeed.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.RegularExpressions;
using JISpeed.Core.Entities.Common;
using Microsoft.AspNetCore.Identity;
using JISpeed.Core.Configurations;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace JISpeed.Application.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SmtpSettings? _smtpSettings;

        public EmailService(
            ILogger<EmailService> logger,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration )
        {
            _logger = logger;
            _userManager = userManager;
            _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        }

        public async Task<string?> SendVerificationEmailAsync(ApplicationUser user)
        {
            try
            {
                // 1. 生成邮箱验证Token并编码
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                _logger.LogInformation("Token生成成功:{Token}",token);
                var encodedToken = WebUtility.UrlEncode(token);
                _logger.LogInformation("Token编码成功:{EncodedToken}",encodedToken);


                // 2. 构造验证链接
                string verificationLink =
                    $"{_smtpSettings.FrontendBaseUrl}/verify-email?userId={user.Id}&token={encodedToken}";
                _logger.LogInformation("验证链接构造成功");

                // 3. 构建HTML邮件内容（目前先用text来代替）
                string htmlBody = $"欢迎注册济时达平台！\n\n" +
                                  $"请点击以下链接完成邮箱验证（有效期1小时）：\n" +
                                  $"{verificationLink}\n\n" +
                                  $"如果链接无法点击，请复制上述链接到浏览器地址栏访问。";

                // 4. 使用MimeKit构建邮件消息
                var message = new MimeMessage();
                // 发件人（显示名称 + 邮箱地址）
                message.From.Add(new MailboxAddress(
                    name: _smtpSettings.DisplayName,
                    address: _smtpSettings.FromEmail
                ));
                // 收件人
                message.To.Add(new MailboxAddress(
                    name: user.UserName, // 可留空，仅显示邮箱
                    address: user.Email
                ));
                // 邮件主题
                message.Subject = "请验证您的邮箱";
                // 邮件内容（HTML格式）
                message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

                _logger.LogInformation("邮件内容构建成功");

                // 5. 使用MailKit发送邮件
                using var client = new SmtpClient();
                try
                {
                    // 连接SMTP服务器（支持超时设置）
                    await client.ConnectAsync(
                        host: _smtpSettings.Host,
                        port: _smtpSettings.Port,
                        useSsl: _smtpSettings.EnableSsl,
                        cancellationToken: CancellationToken.None
                    );
                    _logger.LogInformation("已连接到SMTP服务器");

                    // 身份验证（163邮箱需使用授权码）
                    await client.AuthenticateAsync(
                        userName:_smtpSettings.Username,
                        password: _smtpSettings.Password,
                        cancellationToken: CancellationToken.None
                    );
                    _logger.LogInformation("SMTP身份验证成功");

                    // 发送邮件
                    await client.SendAsync(
                        message: message,
                        cancellationToken: CancellationToken.None
                    );
                    _logger.LogInformation("邮件发送成功");

                    // 断开连接
                    await client.DisconnectAsync(quit: true, cancellationToken: CancellationToken.None);
                }
                finally
                {
                    // 确保即使发送失败也断开连接
                    if (client.IsConnected)
                        await client.DisconnectAsync(quit: true, cancellationToken: CancellationToken.None);
                }

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发送验证邮件到 {user.Email} 失败");
                return null;
            }
        }
    

    public async Task<bool> IsValidEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch{
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }
    }
}
    
