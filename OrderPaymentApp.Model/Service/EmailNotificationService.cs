using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentApp.Model.Service
{
    /// <summary>
    /// сервис уведомления через Email.
    /// </summary>
    public class EmailNotificationService : INotificationService
    {
        /// <summary>
        /// Отправить уведомление через Email.
        /// </summary>
        /// <param name="adress">Адрес.</param>
        /// <param name="subject">Тема</param>
        /// <param name="message">Сообщение</param>
        /// <returns>Асинхронный результат отправления</returns>
        public async Task Send(string adress, string subject, string message)
        {
            var email_message = new MailMessage();
            email_message.From = new MailAddress("service@orderpaymentapp.com");
            email_message.To.Add(adress);

            email_message.Subject = subject;
            email_message.Body = message;

            using (var client = new SmtpClient("mail.svel.ru", 25))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                await client.SendMailAsync(email_message);
            }
        }
    }
}
