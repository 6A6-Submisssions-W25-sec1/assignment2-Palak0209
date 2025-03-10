using EmailConsoleApp.Interfaces;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MauiEmail.Models;
using MimeKit;

namespace MauiEmail.Services
{
    internal class EmailService : IEmailService
    {
        private readonly IMailConfig _config;
        private SmtpClient _smtpClient;
        private ImapClient _imapClient;

        public EmailService(IMailConfig config)
        {
            _config = config;
            _smtpClient = new SmtpClient();
            _imapClient = new ImapClient();
        }
        public async Task StartSendClientAsync()
        {
            try
            {
                if (!_smtpClient.IsConnected)
                    await _smtpClient.ConnectAsync(_config.SendHost, _config.SendPort, _config.SendSocketOptions);

                if (!_smtpClient.IsAuthenticated)
                    await _smtpClient.AuthenticateAsync(_config.EmailAddress, _config.Password);

                Console.WriteLine("SMTP Client Connected & Authenticated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP Connection Error: {ex.Message}");
            }
        }
        public async Task DisconnectSendClientAsync()
        {
            if (_smtpClient.IsConnected)
            {
                await _smtpClient.DisconnectAsync(true);
                Console.WriteLine("SMTP Client Disconnected.");
            }
        }
        public async Task StartRetreiveClientAsync()
        {
            try
            {
                if (!_imapClient.IsConnected)
                    await _imapClient.ConnectAsync(_config.ReceiveHost, _config.ReceivePort, _config.ReceiveSocketOptions);

                if (!_imapClient.IsAuthenticated)
                    await _imapClient.AuthenticateAsync(_config.EmailAddress, _config.Password);

                Console.WriteLine("IMAP Client Connected & Authenticated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IMAP Connection Error: {ex.Message}");
            }
        }
        public async Task DisconnectRetreiveClientAsync()
        {
            if (_imapClient.IsConnected)
            {
                await _imapClient.DisconnectAsync(true);
                Console.WriteLine("IMAP Client Disconnected.");
            }
        }
        public async Task SendMessageAsync(MimeMessage message)
        {
            try
            {
                if (!_smtpClient.IsConnected || !_smtpClient.IsAuthenticated)
                    await StartSendClientAsync();

                await _smtpClient.SendAsync(message);
                await _imapClient.DisconnectAsync(true);

                Console.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Sending Email: {ex.Message}");
            }
        }
        public async Task<IEnumerable<MimeMessage>> DownloadAllEmailsAsync()
        {
            try
            {
                if (!_imapClient.IsConnected || !_imapClient.IsAuthenticated)
                    await StartRetreiveClientAsync();

                var inbox = _imapClient.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);
                var uids = await inbox.SearchAsync(MailKit.Search.SearchQuery.All);

                List<MimeMessage> messages = new List<MimeMessage>();

                foreach (var uid in uids)
                {
                    var message = await inbox.GetMessageAsync(uid);
                    messages.Add(message);
                    Console.WriteLine($"UID: {uid}, From: {message.From}, Subject: {message.Subject}, Date: {message.Date}\n");
                }

                await _imapClient.DisconnectAsync(true);

                Console.WriteLine($"Retrieved {messages.Count} emails.");
                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Retrieving Emails: {ex.Message}");
                return new List<MimeMessage>();
            }
        }
        public async Task DeleteMessageAsync(int uid)
        {
            try
            {
                if (!_imapClient.IsConnected || !_imapClient.IsAuthenticated)
                    await StartRetreiveClientAsync();

                var inbox = _imapClient.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                await inbox.AddFlagsAsync(uid, MessageFlags.Deleted, true);
                await inbox.ExpungeAsync();
                await _imapClient.DisconnectAsync(true);

                Console.WriteLine($"Email with UID {uid} deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Deleting Email: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ObservableMessage>> FetchAllMessages()
        {
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var summaries = await inbox.FetchAsync(0, -1, MessageSummaryItems.Envelope | MessageSummaryItems.Flags);

            var messages = summaries.Select(summary => new ObservableMessage(summary)).ToList();

            await _imapClient.DisconnectAsync(true);
            return messages;
        }

        public async Task<MimeMessage> GetMessageAsync(UniqueId uid)
        {
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var message = await inbox.GetMessageAsync(uid);

            await _imapClient.DisconnectAsync(true);
            return message;
        }

        public async Task MarkAsReadAsync(UniqueId uid)
        {
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite);

            await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);

            await _imapClient.DisconnectAsync(true);
        }

        public async Task MarkAsFavouriteAsync(UniqueId uid)
        {
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite);

            await inbox.AddFlagsAsync(uid, MessageFlags.Flagged, true);

            await _imapClient.DisconnectAsync(true);
        }

        public async Task<IEnumerable<UniqueId>> SearchEmailsAsync(string query)
        {
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var searchResults = await inbox.SearchAsync(SearchQuery.SubjectContains(query)
                .Or(SearchQuery.BodyContains(query))
                .Or(SearchQuery.FromContains(query)));

            await _imapClient.DisconnectAsync(true);
            return searchResults;
        }
    }
}
