using OrderPaymentApp.Model.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;

namespace OrderPaymentApp.Model.Repository
{
    /// <summary>
    /// Репозиторий для работы с заказами.
    /// </summary>
    internal class OrderRepository : RepositoryBase
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="connection_string">Строка соединения с БД.</param>
        internal OrderRepository(string connection_string) : base(connection_string, "OrderPaymentApp")
        {
        }

        /// <summary>
        /// Получение заказа по номеру.
        /// </summary>
        /// <param name="orderNumber">Номер заказа.</param>
        /// <returns>заказ.</returns>
        internal Order GetOrderByNumber(string orderNumber)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.QueryFirstOrDefault<Order>(@"select * from OrderPayment.Orders where Number = @orderNumber", new { orderNumber });
            }
        }


        /// <summary>
        /// Получение заявки по ID запроса на оплату.
        /// </summary>
        /// <param name="paymentRequestID">ID запроса на оплату.</param>
        /// <returns>Заявка.</returns>
        internal Order GetOrderByPaymentRequestID(long paymentRequestID)
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.QueryFirstOrDefault<Order>(@"select * from OrderPayment.GetOrderByPaymentRequestID(@paymentRequestID)", new { paymentRequestID });
            }
        }

        /// <summary>
        /// Создание запроса на оплату.
        /// </summary>
        /// <param name="orderID">ID заявки.</param>
        /// <returns>ID запроса на оплату.</returns>
        internal long CreatePaymentRequest(int orderID)
        {
            using (IDbConnection conn = GetConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@OrderID", orderID, DbType.Int32);
                parameters.Add("@PaymentRequestID", dbType: DbType.Int64, direction: ParameterDirection.Output);

                conn.Execute("OrderPayment.CreatePaymentRequest", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<long>("@PaymentRequestID");
            }
        }

        /// <summary>
        /// Завершить оплату.
        /// </summary>
        /// <param name="orderPayment">Запрос на оплату</param>
        /// <returns>Асинхронный результат.</returns>
        internal void CompletePaymentRequest(long paymentRequestID)
        {
            using (IDbConnection conn = GetConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@PaymentRequestID", paymentRequestID, dbType: DbType.Int64);

                conn.Execute("OrderPayment.CompletePaymentRequest", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
