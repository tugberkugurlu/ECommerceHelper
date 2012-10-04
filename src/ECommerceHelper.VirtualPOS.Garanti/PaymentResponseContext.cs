using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECommerceHelper.VirtualPOS.Garanti.Descriptor.Response;

namespace ECommerceHelper.VirtualPOS.Garanti {

    public class PaymentResponseContext {

        public PaymentResponseCode ResponseCode { get; set; }

        public PaymentResponseDescriptor PaymentResponseDescriptor { get; set; }
        public ICollection<ValidationResult> ValidationResults { get; set; }
    }
}