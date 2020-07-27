using OrderPaymentApp.Model.DataModel;
using System.Threading.Tasks;

namespace OrderPaymentApp.Model.Service
{
    /// <summary>
    /// Сервис работы с Заявками.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Завершить оплату.
        /// </summary>
        /// <param name="orderPayment">Запрос на оплату</param>
        /// <returns>Асинхронный результат.</returns>
        Task CompletePaymentRequest(OrderPayment orderPayment);

        /// <summary>
        /// создание запроса на оплату.
        /// </summary>
        /// <param name="orderID">ID заявки.</param>
        /// <returns>ID запроса на оплату.</returns>
        long CreatePaymentRequest(int orderID);

        /// <summary>
        /// Получение заявки по номеру.
        /// </summary>
        /// <param name="orderNumber">Номер заявки.</param>
        /// <returns>Заявка.</returns>
        Order GetOrderByNumber(string orderNumber);

        /// <summary>
        /// Получение заявки по ID запроса на оплату.
        /// </summary>
        /// <param name="paymentRequestID">ID запроса на оплату.</param>
        /// <returns>Заявка.</returns>
        Order GetOrderByPaymentRequestID(long paymentRequestID);
    }
}