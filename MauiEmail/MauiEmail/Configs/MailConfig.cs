using EmailConsoleApp.Interfaces;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiEmail.Configs
{
    public class MailConfig : IMailConfig
    {
        public string EmailAddress { get; set; } = "palakdummy02@gmail.com";
        public string Password { get; set; } = "xiyq bhfa kyjq ayxs";
        public string ReceiveHost { get; set; } = "imap.gmail.com";
        public SecureSocketOptions ReceiveSocketOptions { get; set; } = SecureSocketOptions.SslOnConnect;
        public int ReceivePort { get; set; } = 993;
        public string SendHost { get; set; } = "smtp.gmail.com";
        public int SendPort { get; set; } = 587;
        public SecureSocketOptions SendSocketOptions { get; set; } = SecureSocketOptions.StartTls;
    }
}
