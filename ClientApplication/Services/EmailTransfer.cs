using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ClientApplication.Services
{
    public interface IEmailTransfer : System.IAsyncDisposable
    {
        public sealed class EmailAccount : object
        {
            public string Login { get; set; } = default!;
            public string Password { get; set; } = default!;
        }
        public enum MessageType : sbyte { Update, Registration, Deleted }
        public Task SendMessage(string messageText, string address);
        public Task SendMessage(IEmailTransfer.MessageType messageType, string address);
    }

    public partial class EmailTransfer : System.Object, IEmailTransfer
    {
        protected IEmailTransfer.EmailAccount EmailAccount { get; set; } = default!;
        public EmailTransfer(IOptions<IEmailTransfer.EmailAccount> emailAccount) : base() 
        { this.EmailAccount = emailAccount.Value; }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public virtual Task SendMessage(string messageText, string address)
        {
            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(this.EmailAccount.Login, this.EmailAccount.Password),
                EnableSsl = true
            };
            var transferFrom = new MailAddress(this.EmailAccount.Login, "Информация о контакте");
            using var emailMessage = new MailMessage(transferFrom, new MailAddress(address))
            {
                Subject = "Записная книжка", Body = messageText,
            };
            smtpClient.SendAsync(emailMessage, "token"); return Task.CompletedTask;
        }
        public virtual async Task SendMessage(IEmailTransfer.MessageType messageType, string address)
        {
            await this.SendMessage(messageType switch 
            {
                IEmailTransfer.MessageType.Registration => "Вы успешно зарегистрировались в нашем приложении",
                IEmailTransfer.MessageType.Update => "Данные вашего профиля были обновлены",
                IEmailTransfer.MessageType.Deleted => "Ваш аккаунт был удалён из системы, приносим извинения :)",
            _=> "Ошибка" }, address);
        }
    }
}
