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

        [StringLength(19, MinimumLength = 15)]
        public string CreditCardNumber { get; set; }
        
        public int CCExpireDateYear { get; set; }

        [Range(1, 12)]
        public int CCExpireDateMonth { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 3)]
        public string CVV2 { get; set; }

        public CurrencyCode CurrencyCode { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}