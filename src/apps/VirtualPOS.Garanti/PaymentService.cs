using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ECommerce.VirtualPOS.Garanti.Descriptor;
using System.Net.Http.Formatting;
using ECommerce.VirtualPOS.Garanti.Descriptor.Response;
using System.IO;
using ECommerce.VirtualPOS.Garanti.Descriptor.Request;

namespace ECommerce.VirtualPOS.Garanti {

    public class PaymentService : IPaymentService {

        const string _version = "v0.00";

        private readonly string _merchantId;
        private readonly string _terminalId;
        private readonly string _provisionUserId;
        private readonly string _terminalPassword;
        private readonly string _userId;
        private readonly Mode _mode;
        private readonly CardholderPresentCode _cardholderPresentCode;

        private readonly HttpClient _httpClient;

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
            : this(merchantId, terminalId, provisionUserId, terminalPassword, userId, Mode.PROD, CardholderPresentCode.Normal) {
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

            _httpClient = new HttpClient();
        }

        public async Task<PaymentResponseContext> ProcessSaleAsync(PaymentRequestContext paymentRequest) {

            if (paymentRequest == null)
                throw new ArgumentNullException("paymentRequest");

            //validate the PaymentRequestContext instance comming in
            var ctxValResults = validatePaymentRequestContext(paymentRequest);
            if (ctxValResults.Any()) {

                return new PaymentResponseContext {

                    ResponseCode = PaymentResponseCode.InvalidPaymentRequestContext,
                    ValidationResults = ctxValResults
                };
            }

            //PaymentRequestContext instance is valid. Continue.
            var paymentServiceDescriptor = createPaymentServiceDescriptor(paymentRequest);
            var requestXML = seserializePaymentRequest(paymentServiceDescriptor);

            return await processPaymentRequest(requestXML);
        }

        //private helpers
        private ICollection<ValidationResult> validatePaymentRequestContext(
            PaymentRequestContext paymentRequest) {

            if (paymentRequest == null)
                throw new ArgumentNullException("paymentRequest");

            ValidationContext validationContext = 
                new ValidationContext(paymentRequest, null, null);

            List<ValidationResult> valResults = new List<ValidationResult>();
            Validator.TryValidateObject(paymentRequest, validationContext, valResults, true);

            return valResults;
        }

        public PaymentServiceDescriptor createPaymentServiceDescriptor(PaymentRequestContext paymentRequest) {

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
                    Number = paymentRequest.CreditCardNumber,
                    ExpireDate = PaymentUtility.EncodeExpireDate(
                        paymentRequest.CCExpireDateYear, paymentRequest.CCExpireDateMonth
                    ),
                    CVV2 = string.Empty
                },
                Transaction = new TransactionRequest {
                    CardholderPresentCodeDigit = _cardholderPresentCode.GetHashCode(),
                    AmountString = PaymentUtility.EncodeDecimalPaymentAmount(paymentRequest.PaymentAmount),
                    CurrencyCodeDigit = paymentRequest.CurrencyCode.GetHashCode(),
                    Type = OperationType.Sales.ToString().ToUpper(),
                    InstallmentCnt = string.Empty
                },
                Order = new OrderRequest { 
                    GroupID = string.Empty,
                    OrderID = string.Empty
                }
            };
        }

        public string seserializePaymentRequest(PaymentServiceDescriptor paymentServiceDescriptor) {

            XmlSerializer serializer = new XmlSerializer(typeof(PaymentServiceDescriptor));

            XmlSerializerNamespaces emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add(string.Empty, string.Empty);

            StringBuilder output = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(output, new XmlWriterSettings { OmitXmlDeclaration = true })) {
                
                serializer.Serialize(writer, paymentServiceDescriptor, emptyNamespace);
            }

            return output.ToString();
        }

        private async Task<PaymentResponseContext> processPaymentRequest(string requestXML) {

            var formattedContent = string.Format("data={0}", requestXML);

            //NOTE: Garanti Bank accepts the request as application/x-www-form-urlencoded
            var content = new StringContent(formattedContent);
            content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            var response = await _httpClient.PostAsync(
                Constants.BASE_VIRTUAL_POS_REQUEST_ADDRESS, content
            );

            var stringContent = await response.Content.ReadAsAsync<string>();
            var paymentResponseCtx = deseserializePaymentResponse(stringContent);

            return paymentResponseCtx;
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

        public void Dispose() {

            _httpClient.Dispose();
        }
    }
}