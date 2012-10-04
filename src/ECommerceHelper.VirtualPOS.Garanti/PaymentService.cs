using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ECommerceHelper.VirtualPOS.Garanti.Descriptor.Response;
using ECommerceHelper.VirtualPOS.Garanti.Descriptor.Request;
using System.Collections.ObjectModel;

namespace ECommerceHelper.VirtualPOS.Garanti {

    public class PaymentService : IPaymentService {

        const string _version = "v0.00";

        private readonly string _merchantId;
        private readonly string _terminalId;
        private readonly string _provisionUserId;
        private readonly string _terminalPassword;
        private readonly string _userId;
        private readonly Mode _mode;
        private readonly CardholderPresentCode _cardholderPresentCode;

        public PaymentService(
            string merchantId, string terminalId, string provisionUserId, 
            string terminalPassword) : this(merchantId, terminalId, provisionUserId, terminalPassword, "INTSALE") {
        }

        public PaymentService(
            string merchantId, string terminalId, string provisionUserId,
            string terminalPassword, string userId) : this(merchantId, terminalId, provisionUserId, terminalPassword, userId, Mode.PROD) {
        }

        public PaymentService(
            string merchantId, string terminalId, string provisionUserId,
            string terminalPassword, string userId, Mode mode)
            : this(merchantId, terminalId, provisionUserId, terminalPassword, userId, mode, CardholderPresentCode.Normal) {
        }

        public PaymentService(
            string merchantId, string terminalId, string provisionUserId,
            string terminalPassword, string userId, Mode mode, CardholderPresentCode cardholderPresentCode) {

            if(string.IsNullOrEmpty(merchantId))
                throw new ArgumentNullException("merchantId");

            if (string.IsNullOrEmpty(terminalId))
                throw new ArgumentNullException("terminalId");

            if (string.IsNullOrEmpty(provisionUserId))
                throw new ArgumentNullException("provisionUserId");

            if (string.IsNullOrEmpty(terminalPassword))
                throw new ArgumentNullException("terminalPassword");

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId");

            if(!Enum.IsDefined(typeof(Mode), mode))
                throw new ArgumentNullException("mode");

            //NOTE: 3D is not supported for now
            if (cardholderPresentCode == CardholderPresentCode.ThreeD)
                throw new NotSupportedException("3D transactions is not supported for now.");

            _merchantId = merchantId;
            _terminalId = terminalId;
            _provisionUserId = provisionUserId;
            _terminalPassword = terminalPassword;
            _userId = userId;
            _mode = mode;
            _cardholderPresentCode = cardholderPresentCode;
        }

        public Task<PaymentResponseContext> ProcessSaleAsync(PaymentRequestContext paymentRequest) {

            if (paymentRequest == null)
                throw new ArgumentNullException("paymentRequest");

            //validate the PaymentRequestContext instance comming in
            ICollection<ValidationResult> ctxValResults = validatePaymentRequestContext(paymentRequest);
            if (ctxValResults.Any()) {

                var invalidPaymentResponseCtx = new PaymentResponseContext {

                    ResponseCode = PaymentResponseCode.InvalidPaymentRequestContext,
                    ValidationResults = ctxValResults
                };

                return TaskHelpers.FromResult(invalidPaymentResponseCtx);
            }

            //PaymentRequestContext instance is valid. Continue.
            var paymentServiceDescriptor = createPaymentServiceDescriptor(paymentRequest);
            var requestXML = seserializePaymentRequest(paymentServiceDescriptor);

            return processPaymentRequest(requestXML);
        }

        //private helpers
        private ICollection<ValidationResult> validatePaymentRequestContext(
            PaymentRequestContext paymentRequest) {

            if (paymentRequest == null)
                throw new ArgumentNullException("paymentRequest");

            ValidationContext validationContext = 
                new ValidationContext(paymentRequest, null, null);

            var valResults = new Collection<ValidationResult>();
            Validator.TryValidateObject(paymentRequest, validationContext, valResults, true);

            return valResults;
        }

        private PaymentServiceDescriptor createPaymentServiceDescriptor(PaymentRequestContext paymentRequest) {

            return new PaymentServiceDescriptor {

                ModeString = _mode.ToString(),
                Version = _version,
                Terminal = new Terminal {
                    MerchantID = _merchantId,
                    ID = _terminalId,
                    ProvUserID = _provisionUserId,
                    UserID = _userId,
                    HashData = PaymentUtility.GenerateHASHedData(
                        _terminalId,
                        paymentRequest.CreditCardNumber.ToString(),
                        PaymentUtility.EncodeDecimalPaymentAmount(
                            paymentRequest.PaymentAmount
                        ),
                        _terminalPassword
                    )
                },
                Customer = new Customer {
                    EmailAddress = paymentRequest.CustomerEmail,
                    IPAddress = paymentRequest.IpAddress
                },
                Card = new Card {
                    Number = paymentRequest.CreditCardNumber.ToString(),
                    ExpireDate = PaymentUtility.EncodeExpireDate(
                        paymentRequest.CCExpireDateMonth, paymentRequest.CCExpireDateYear
                    ),
                    //TODO: This is a temp fix. Make this better.
                    CVV2 = (paymentRequest.CVV2.ToString().Length < 3) ? string.Format("0{0}", paymentRequest.CVV2.ToString()) : paymentRequest.CVV2.ToString()
                },
                Transaction = new TransactionRequest {
                    CardholderPresentCodeDigit = _cardholderPresentCode.GetHashCode(),
                    AmountString = PaymentUtility.EncodeDecimalPaymentAmount(paymentRequest.PaymentAmount),
                    CurrencyCodeDigit = paymentRequest.CurrencyCode.GetHashCode(),
                    Type = OperationType.Sales.ToString().ToLower(),
                    InstallmentCnt = string.Empty
                },
                Order = new OrderRequest { 
                    GroupID = string.Empty,
                    OrderID = string.Empty
                }
            };
        }

        private string seserializePaymentRequest(PaymentServiceDescriptor paymentServiceDescriptor) {

            XmlSerializer serializer = new XmlSerializer(typeof(PaymentServiceDescriptor));

            XmlSerializerNamespaces emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add(string.Empty, string.Empty);

            StringBuilder output = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(output, new XmlWriterSettings { OmitXmlDeclaration = true })) {
                
                serializer.Serialize(writer, paymentServiceDescriptor, emptyNamespace);
            }

            return output.ToString();
        }

        private Task<PaymentResponseContext> processPaymentRequest(string requestXML) {

            //NOTE: Garanti Bank accepts the request as application/x-www-form-urlencoded

            TaskCompletionSource<PaymentResponseContext> tcs = new TaskCompletionSource<PaymentResponseContext>();
            string formattedContent = string.Format("data={0}", requestXML);
            StringContent content = new StringContent(formattedContent);
            content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            HttpClient httpClient = new HttpClient();

            return httpClient.PostAsync(Constants.BASE_VIRTUAL_POS_REQUEST_ADDRESS, content).Then<HttpResponseMessage, PaymentResponseContext>(response => {

                // Note the best way to handle this but will do the work
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().Then<string, PaymentResponseContext>(stringResult => {

                    var paymentResponseCtx = deseserializePaymentResponse(stringResult);

                    try {

                        tcs.SetResult(paymentResponseCtx);
                    }
                    catch (Exception ex) {

                        tcs.SetException(ex);
                    }

                    return tcs.Task;

                }, runSynchronously: true);

            }, runSynchronously: true).Catch<PaymentResponseContext>(info => {

                tcs.SetException(info.Exception);
                return new CatchInfoBase<Task<PaymentResponseContext>>.CatchResult { Task = tcs.Task };

            }).Finally(() => httpClient.Dispose(), runSynchronously: true);
        }

        private PaymentResponseContext deseserializePaymentResponse(string paymentResponseString) {

            XmlSerializer serializer = new XmlSerializer(typeof(PaymentResponseDescriptor));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(paymentResponseString))) {

                var paymentResponseDescriptor = (PaymentResponseDescriptor)serializer.Deserialize(ms);

                return new PaymentResponseContext { 
                    ResponseCode = paymentResponseDescriptor.Transaction.Response.ResponseCode,
                    PaymentResponseDescriptor = paymentResponseDescriptor
                };
            }
        }
    }
}