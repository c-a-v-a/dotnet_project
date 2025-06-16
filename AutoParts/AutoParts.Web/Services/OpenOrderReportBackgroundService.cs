using AutoParts.Web.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AutoParts.Web.Services
{
    public class OpenOrderReportBackgroundService : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<OpenOrderReportBackgroundService> _logger;
        private readonly IServiceProvider _services;
        private readonly ReportEmailSettings _emailSettings;

        public OpenOrderReportBackgroundService(IServiceProvider services, IWebHostEnvironment env, ILogger<OpenOrderReportBackgroundService> logger, IOptions<ReportEmailSettings> emailSettings)
        {
            _env = env;
            _services = services;
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("⏳ Czekam 15 sekund...");
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

                    Console.WriteLine("📄 Pobieram PDF...");

                    // 🔐 Ustawienie handlera ignorującego certyfikat (TYLKO DO TESTÓW!)
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };

                    using var client = new HttpClient(handler);
                    var pdfBytes = await client.GetByteArrayAsync("https://localhost:7252/Reports/OpenOrdersPdf");

                    Console.WriteLine("📬 Tworzę i wysyłam e-mail...");

                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse(_emailSettings.Sender));
                    message.To.Add(MailboxAddress.Parse(_emailSettings.Recipient));
                    message.Subject = "Raport otwartych napraw";

                    var builder = new BodyBuilder
                    {
                        TextBody = "W załączniku znajduje się automatyczny raport otwartych napraw."
                    };
                    builder.Attachments.Add("raport-otwarte-naprawy.pdf", pdfBytes, ContentType.Parse("application/pdf"));
                    message.Body = builder.ToMessageBody();

                    using var smtp = new SmtpClient();
                    await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);

                    Console.WriteLine($"✅ E-mail wysłany o {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Błąd:");
                    Console.WriteLine(ex.ToString());
                }
            }
        }

    }
}
