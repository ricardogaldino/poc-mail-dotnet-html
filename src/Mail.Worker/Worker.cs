using System.Net;
using System.Net.Mail;

namespace Mail.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var html = File.ReadAllText("Views/index.html");
                html = html.Replace("##message##", "Lorem ipsum");

                var mail = new MailMessage
                {
                    To = {new MailAddress("to@mail.com")},
                    From = new MailAddress("from@mail.com"),
                    Subject = "Subject",
                    Body = html
                };

                var smtp = new SmtpClient
                {
                    Host = "smtp.mail.com",
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential("user", "password"),
                    EnableSsl = true,
                    Port = 465
                };

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Error: {ex.Message}");
            }
            finally
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
    }
}