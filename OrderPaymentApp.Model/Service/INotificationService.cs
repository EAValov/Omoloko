using System.Threading.Tasks;

namespace OrderPaymentApp.Model.Service
{
    /// <summary>
    /// Работа с уведомлениями.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Отправить уведомление.
        /// </summary>
        /// <param name="adress">Адрес.</param>
        /// <param name="subject">Тема</param>
        /// <param name="message">Сообщение</param>
        /// <returns>Асинхронный результат отправления</returns>
        Task Send(string adress, string subject, string message);
    }
}