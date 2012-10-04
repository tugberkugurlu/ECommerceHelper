using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.VirtualPOS.Garanti.Descriptor.Response {

    [Serializable]
    [XmlRoot("GVPSResponse")]
    public class PaymentResponseDescriptor {

        [XmlElement("Mode")]
        public string ModeString { get; set; }

        [XmlElement("Order", typeof(OrderResponse))]
        public OrderResponse Order { get; set; }

        [XmlElement("Transaction", typeof(TransactionResponse))]
        public TransactionResponse Transaction { get; set; }
    }
}