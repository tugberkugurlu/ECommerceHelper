using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using ECommerce.Common;

namespace ECommerce.VirtualPOS.Garanti {

    /// <summary>
    /// Context class for the payment. Holds the payment necessary details.
    /// </summary>
    public class PaymentRequestContext {

        [Required]
        public string CustomerEmail { get; set; }
        [Required]
        public string IpAddress { get; set; }

        public string CreditCardNumber { get; set; }
        public int CCExpireDateYear { get; set; }
        [Range(1, 12)]
        public int CCExpireDateMonth { get; set; }

        public CurrencyCode CurrencyCode { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}