using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Response {

    [Serializable]
    public class TransactionResponse {

        [XmlElement("RetrefNum")]
        public string RetrefNum { get; set; }

        [XmlElement("AuthCode")]
        public string AuthCode { get; set; }

        [XmlElement("BatchNum")]
        public string BatchNum { get; set; }

        [XmlElement("SequenceNum")]
        public string SequenceNum { get; set; }

        [XmlElement("ProvDate")]
        public string ProvDate { get; set; }

        [XmlElement("Response", typeof(Response))]
        public Response Response { get; set; }
    }
}
