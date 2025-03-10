using EmailConsoleApp.Interfaces;
using MauiEmail.Configs;
using MauiEmail.Services;

namespace MauiEmail
{
    public partial class App : Application
    {
        public static IMailConfig EmailConfig { get; } = new MailConfig();
        public static IEmailService EmailService { get; } = new EmailService(EmailConfig);
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
