using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentApp.Model.DataModel
{
    public class OrderPayment : Order
    {
        public long PaymentRequestID { get; set; }
    }
}
