using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using VirtualPOS.Garanti.Descriptor;
using System.Net.Http.Formatting;

namespace VirtualPOS.Garanti {

    public class PaymentService : IPaymentService {

        const string _version = "v0.00";

        private readonly string _merchantId;
        private readonly string _terminalId;
        private readonly string _provisionUserId;
        private readonly string _terminalPassword;
        private readonly string _userId;
        private readonly Mode _mode;

        private readonly HttpClient _httpClient;

        public PaymentService(
            string merchantId, string terminalId, string provisionUserId, 
            string terminalPassword) : this(merchantId, terminalId, provisionUserId, terminalPassword, "INTENET_SALE") {
        }

        public PaymentService(
            string merchantId, string terminalId, string provisionUserId,
            string terminalPassword, string userId) : this(merchantId, terminalId, provisionUserId, terminalPassword, userId, Mode.PROD) {
        }

        public PaymentService(
            string merchantId, string terminalId, string provisionUserId,
            string terminalPassword, string userId, Mode mode) {

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

            _merchantId = merchantId;
            _terminalId = terminalId;
            _provisionUserId = provisionUserId;
            _terminalPassword = terminalPassword;
            _userId = userId;
            _mode = mode;

            _httpClient = new HttpClient();
        }

        public async Task<PaymentResponseContext> ProcessPayment(PaymentRequestContext paymentRequest) {

            if (paymentRequest == null)
                throw new ArgumentNullException("paymentRequest");

            //validate the PaymentRequestContext instance comming in
            var ctxValResults = validatePaymentRequestContext(paymentRequest);
            if (ctxValResults.Any()) {

                return new PaymentResponseContext {

                    PaymentResponseCode = PaymentResponseCode.InvalidPaymentRequestContext,
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
                Transaction = new Transaction {
                    CardholderPresentCodeDigit = CardholderPresentCode.Normal.GetHashCode(),
                    AmountString = PaymentUtility.EncodeDecimalPaymentAmount(paymentRequest.PaymentAmount),
                    CurrencyCodeDigit = paymentRequest.CurrencyCode.GetHashCode(),
                    Type = "Sales",
                    InstallmentCnt = string.Empty
                },
                Order = new Order { 
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

            var content = new StringContent(requestXML);
            content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            var response = await _httpClient.PostAsync(
                Constants.BASE_VIRTUAL_POS_REQUEST_ADDRESS, content
            );

            var stringContent = await response.Content.ReadAsAsync<string>();
            var paymentResponseCtx = deseserializePaymentResponse(stringContent);

            return paymentResponseCtx;
        }

        private PaymentResponseContext deseserializePaymentResponse(string paymentResponseString) {

            throw new NotImplementedException();
        }

        public void Dispose() {

            _httpClient.Dispose();
        }
    }
}