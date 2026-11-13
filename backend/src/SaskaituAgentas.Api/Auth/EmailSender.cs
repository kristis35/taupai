namespace SaskaituAgentas.Api.Auth;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}

/// <summary>
/// Laikina implementacija: laiškas tik įrašomas į žurnalą. Pakeisti tikru SMTP / API siuntėju,
/// kai bus pasirinktas el. pašto tiekėjas.
/// </summary>
public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        logger.LogInformation("El. laiškas (nesiųstas, tik žurnale) → {To}: {Subject}\n{Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
