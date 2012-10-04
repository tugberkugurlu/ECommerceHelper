using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using ECommerceHelper.Common;

namespace ECommerceHelper.VirtualPOS.Garanti {

    /// <summary>
    /// Context class for the sale. Holds the payment necessary details.
    /// </summary>
    public class PaymentRequestContext {

        [Required]
        public string CustomerEmail { get; set; }

        [Required]
        public string IpAddress { get; set; }

        /// <summary>
        /// Credit Card number. This value should be at least 15 and at most 19 digits integer
        /// </summary>
        public long CreditCardNumber { get; set; }
        
        public int CCExpireDateYear { get; set; }

        [Range(1, 12)]
        public int CCExpireDateMonth { get; set; }

        /// <summary>
        /// Credit Card CVV2 security number. This value should be at least 3 and at most 4 digits integer
        /// </summary>
        [Required]
        public int CVV2 { get; set; }

        public CurrencyCode CurrencyCode { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}