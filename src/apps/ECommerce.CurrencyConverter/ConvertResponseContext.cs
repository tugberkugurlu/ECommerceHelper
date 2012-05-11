using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Common;

namespace ECommerce.CurrencyConverter {

    public class ConvertResponseContext {

        public bool IsConvertSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }

        public CurrencyCode ConvertedFrom { get; set; }
        public CurrencyCode ConvertedTo { get; set; }
        public decimal BaseCurrencyRate { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
