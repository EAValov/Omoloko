using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OrderPaymentApp.Model.DataModel
{
    public class Order
    {
        public int OrderID { get; set; }

        public string Number { get; set; }

        public string ClientEmail { get; set; }
    }
}
