using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Response {

    [Serializable]
    public class TransactionResponse {

        public string RetrefNum { get; set; }
        public string AuthCode { get; set; }
        public string BatchNum { get; set; }
        public string SequenceNum { get; set; }
        public string ProvDate { get; set; }
        public string CardNumberMasked { get; set; }

        [XmlElement("CardHolderName")]
        public string CardHolderNameMasked { get; set; }

        public string CardType { get; set; }
        public string HashData { get; set; }

        [XmlElement("Response", typeof(Response))]
        public Response Response { get; set; }
    }
}
