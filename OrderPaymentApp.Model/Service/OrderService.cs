using OrderPaymentApp.Model.DataModel;
using OrderPaymentApp.Model.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentApp.Model.Service
{
    /// <summary>
    /// Сервис работы с Заявками.
    /// </summary>
    public class OrderService : IOrderService
    {
        /// <summary>
        /// Репозиторий для работы с БД заказов.
        /// </summary>
        OrderRepository orderRepository;

        /// <summary>
        /// Сервис работы с уведомлениями.
        /// </summary>
        INotificationService notificationService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="order_connection_string">Строка соединения с БД заказов</param>
        public OrderService(string order_connection_string, INotificationService notificationService)
        {
            this.orderRepository = new OrderRepository(order_connection_string);
            this.notificationService = notificationService;
        }

        /// <summary>
        /// Получение заказа по номеру.
        /// </summary>
        /// <param name="orderNumber">Номер заказа.</param>
        /// <returns>заказ.</returns>
        public Order GetOrderByNumber(string orderNumber)
        {
            try
            {
                return orderRepository.GetOrderByNumber(orderNumber);
            }
            catch (Exception ex)
            {
                throw new Exception($"При получении заказа по номеру {orderNumber} произошла ошибка", ex);
            }
        }

        /// <summary>
        /// Получение заявки по ID запроса на оплату.
        /// </summary>
        /// <param name="paymentRequestID">ID запроса на оплату.</param>
        /// <returns>Заявка.</returns>
        public Order GetOrderByPaymentRequestID(long paymentRequestID)
        {
            try
            {
                return orderRepository.GetOrderByPaymentRequestID(paymentRequestID);
            }
            catch (Exception ex)
            {
                throw new Exception($"При получении заявки по ID запроса на оплату {paymentRequestID} произошла ошибка", ex);
            }
        }

        /// <summary>
        /// создание запроса на оплату.
        /// </summary>
        /// <param name="orderID">ID заявки.</param>
        /// <returns>ID запроса на оплату.</returns>
        public long CreatePaymentRequest(int orderID)
        {
            try
            {
                return orderRepository.CreatePaymentRequest(orderID);
            }
            catch (Exception ex)
            {
                throw new Exception($"При создании запроса на оплату для заявки {orderID} произошла ошибка", ex);
            }
        }

        /// <summary>
        /// Завершить оплату.
        /// </summary>
        /// <param name="orderPayment">Запрос на оплату</param>
        /// <returns>Асинхронный результат.</returns>
        public async Task CompletePaymentRequest(OrderPayment orderPayment)
        {
            try
            {
                orderRepository.CompletePaymentRequest(orderPayment.PaymentRequestID);
                await notificationService.Send(orderPayment.ClientEmail, "Заказ выполнен", $"Заказ {orderPayment.Number} выполнен");
            }
            catch (Exception ex)
            {
                throw new Exception($"При завершении оплаты заявки {orderPayment.Number} произошла ошибка", ex);
            }
        }
    }
}
