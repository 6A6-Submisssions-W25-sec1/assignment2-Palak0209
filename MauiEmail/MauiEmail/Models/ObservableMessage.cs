using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using PropertyChanged;

namespace MauiEmail.Models
{
    public class ObservableMessage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public UniqueId? UniqueId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string? HtmlBody { get; set; }
        public MailboxAddress From { get; set; }
        public List<MailboxAddress> To { get; set; } = new();
        public bool IsRead { get; set; }
        public bool IsFavorite { get; set; }

        public ObservableMessage(IMessageSummary message)
        {
            UniqueId = message.UniqueId;
            Date = message.Envelope.Date ?? DateTimeOffset.MinValue;
            Subject = message.Envelope.Subject ?? "(No Subject)";
            From = (MailboxAddress)message.Envelope.From[0];
            To = message.Envelope.To.Select(addr => (MailboxAddress)addr).ToList();
            IsRead = (message.Flags == MessageFlags.Seen);
            IsFavorite = (message.Flags == MessageFlags.Flagged);
        }

        public ObservableMessage(MimeMessage mimeMessage, UniqueId uniqueId)
        {
            UniqueId = uniqueId;
            Date = mimeMessage.Date;
            Subject = mimeMessage.Subject ?? "(No Subject)";
            Body = mimeMessage.TextBody;
            HtmlBody = mimeMessage.HtmlBody;
            From = mimeMessage.From.Mailboxes.FirstOrDefault() ?? new MailboxAddress("Unknown", "unknown@example.com");
            To = mimeMessage.To.Mailboxes.ToList();
            IsRead = false;
            IsFavorite = false;
        }

        public MimeMessage ToMime()
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(From);
            mimeMessage.To.AddRange(To);
            mimeMessage.Subject = Subject;
            mimeMessage.Body = new TextPart("plain") { Text = Body };
            return mimeMessage;
        }

        public ObservableMessage GetForward()
        {
            return new ObservableMessage(new MimeMessage
            {
                Subject = "FW: " + Subject,
                Body = new TextPart("plain")
                {
                    Text = $"——— Forwarded message ———\nFrom: {From}\nTo: {string.Join(", ", To)}\n\n{Body}"
                }
            }, UniqueId ?? UniqueId.Value);
        }
    }
}
