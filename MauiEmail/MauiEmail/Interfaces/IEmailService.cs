using MauiEmail.Models;
using MimeKit;

namespace EmailConsoleApp.Interfaces
{
    public interface IEmailService
    {
        // Connection and authentication
        Task StartSendClientAsync();
        Task DisconnectSendClientAsync();
        Task StartRetreiveClientAsync();
        Task DisconnectRetreiveClientAsync();

        // Emailing functionality
        Task SendMessageAsync(MimeMessage message);
        Task<IEnumerable<MimeMessage>> DownloadAllEmailsAsync();
        Task DeleteMessageAsync(int uid);
        Task<IEnumerable<ObservableMessage>?> FetchAllMessages();
    }
}
