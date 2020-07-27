using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentApp.Model.DataModel;
using OrderPaymentApp.Model.Service;

namespace OrderPaymentApp.Controllers
{
    /// <summary>
    /// Контроллер работы с запросами на оплату.
    /// </summary>
    public class PaymentRequestController : Controller
    {
        IOrderService _orderService;// = new OrderService("Server=57-RULESTREAM-1\\RULESTREAM_TEST; user id=order_payment_service; password=K3pMJP");

        /// <summary>
        /// Контроллер.
        /// </summary>
        /// <param name="orderService"><see cref="_orderService"/></param>
        public PaymentRequestController(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        /// <summary>
        /// Представление для Id запроса.
        /// </summary>
        /// <param name="id">ID запроса на оплату.</param>
        /// <returns>Представление.</returns>
        public IActionResult Request(long id)
        {
            ViewBag.PaymentRequestID = id;
            return View("Index");
        }

        /// <summary>
        /// Представление завершения оплаты.
        /// </summary>
        /// <param name="orderPayment">Запрос на оплату заказа</param>
        /// <returns>Представление</returns>
        public IActionResult Complete(OrderPayment orderPayment)
        {
            return View("PaymentComplete", orderPayment);
        }

        /// <summary>
        /// Получение заказа по ID запроса на оплату.
        /// </summary>
        /// <param name="paymentRequestID">ID запроса на оплату</param>
        /// <returns>Заказ</returns>
        [HttpGet]
        public ActionResult<Order> GetOrderByPaymentRequestID(long paymentRequestID)
        {
            try
            {
                var order = _orderService.GetOrderByPaymentRequestID(paymentRequestID);

                if (order is null)
                    return NotFound($"Заказ с номером платежа {paymentRequestID} не найден");

                return order;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Завершение запроса на оплату(оплата заказа).
        /// </summary>
        /// <param name="orderPayment">Запрос на оплату.</param>
        /// <returns>Редирект на страницу <see cref="Complete"/></returns>
        [HttpPost]
        public async Task<ActionResult> CompletePaymentRequest(OrderPayment orderPayment)
        {
            try
            {
                await _orderService.CompletePaymentRequest(orderPayment);

                return Json(new { result = "Redirect", url = Url.Action("Complete", "PaymentRequest", orderPayment) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
