using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ECommerce.VirtualPOS.Garanti {

    public class PaymentResponseContext {

        public PaymentResponseCode PaymentResponseCode { get; set; }
        public ICollection<ValidationResult> ValidationResults { get; set; }
    }
}