using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualPOS.Garanti {

    public interface IPaymentService : IDisposable {

        Task<PaymentResponseContext> ProcessPayment(PaymentRequestContext paymentRequest);
    }
}