using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderPaymentApp.Model.DataModel;
using OrderPaymentApp.Model.Service;
using System.Transactions;

namespace OrderPaymentApp.Tests
{
    [TestClass]
    public class OrderServiceTests
    {
        static OrderService _orderService;

        static OrderServiceTests()
        {
            _orderService = new OrderService("Строка соединения с БД", new EmailNotificationService()); // нужен мок, чтобы не отправлять емайлы во время теста
        }

        [TestMethod]
        public void GetOrderByNumber_ValidNumber_Order()
        {
            var number = "200723-111111";

            var result = _orderService.GetOrderByNumber(number);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Number == number);
            Assert.IsNotNull(result.OrderID);
        }

        [TestMethod]
        public void CreatePaymentRequest_OrderId_PaymentREquestID()
        {
            using(var scope = new TransactionScope())
            {
                var number = "200723-111111";

                var order = _orderService.GetOrderByNumber(number);

                var result = _orderService.CreatePaymentRequest(order.OrderID);

                Assert.IsTrue(result != 0);
            }
        }
    }
}
