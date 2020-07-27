using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPaymentApp.Model.DataModel;
using OrderPaymentApp.Model.Service;
using System;
using System.Threading.Tasks;

namespace OrderPaymentApp.Controllers
{
    /// <summary>
    /// Контроллер заказов.
    /// </summary>
    public class OrderController : Controller
    {
        /// <summary>
        /// Сервис работы с Заявками.
        /// </summary>
        readonly IOrderService _orderService;// = new OrderService("Server=57-RULESTREAM-1\\RULESTREAM_TEST; user id=order_payment_service; password=K3pMJP");

        /// <summary>
        /// Сервис работы с уведомлениями.
        /// </summary>
        readonly INotificationService _notificationService; // = new EmailNotificationService();

        /// <summary>
        /// Доступ к Http контектсту.
        /// </summary>
        readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="httpContextAccessor"><see cref="_httpContextAccessor"/></param>
        /// <param name="orderService"><see cref="_orderService"/></param>
        /// <param name="notificationService"><see cref="_notificationService"/></param>
        public OrderController(IHttpContextAccessor httpContextAccessor, IOrderService orderService, INotificationService notificationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _notificationService = notificationService;
        }
        
        /// <summary>
        /// Представление.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Создание запроса на оплату.
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Уведомление об успехе / ошибка.</returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreatePaymentRequest(Order order)
        {
            try
            {
                var request_number = _orderService.CreatePaymentRequest(order.OrderID);
                var builder = new UriBuilder("http");
                builder.Host = _httpContextAccessor.HttpContext.Request.Host.Host;
                builder.Port = _httpContextAccessor.HttpContext.Request.Host.Port.Value;
                builder.Path = $"PaymentRequest/Request/{request_number}";

                var request_uri = builder.Uri;

                await _notificationService.Send(order.ClientEmail, $"Ссылка на оплату заказа {order.Number}", request_uri.ToString());

                return $"Ссылка на оплату отправлена на email {order.ClientEmail}";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получение заявки по номеру.
        /// </summary>
        /// <param name="orderNumber">Номер заявки.</param>
        /// <returns>Заявка.</returns>
        [HttpGet]
        public ActionResult<Order> GetOrderByNumber(string orderNumber)
        {
            try
            {
                var order = _orderService.GetOrderByNumber(orderNumber);

                if (order is null)
                    return NotFound($"Заказ с номером {orderNumber} не найден");

                return order;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
