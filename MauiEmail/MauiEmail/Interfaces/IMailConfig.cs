using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsoleApp.Interfaces
{
    public interface IMailConfig
    {
        string EmailAddress { get; }
        string Password { get; }
        string ReceiveHost { get; }
        SecureSocketOptions ReceiveSocketOptions { get; }
        int ReceivePort { get; }
        string SendHost { get; }
        int SendPort { get; }
        SecureSocketOptions SendSocketOptions { get; }

        //string OAuth2ClientId { get; set; }
        //string OAuth2ClientSecret { get; set; }
        //public string OAuth2RefreshToken { get; set; }
    }
}
