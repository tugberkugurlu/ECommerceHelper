using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    [XmlRoot("GVPSRequest")]
    public class PaymentServiceDescriptor {

        [XmlElement("Mode")]
        public string ModeString { get; set; }

        public string Version { get; set; }

        [XmlElement("Terminal", typeof(Terminal))]
        public Terminal Terminal { get; set; }

        [XmlElement("Customer", typeof(Customer))]
        public Customer Customer { get; set; }

        [XmlElement("Card", typeof(Card))]
        public Card Card { get; set; }

        [XmlElement("Order", typeof(OrderRequest))]
        public OrderRequest Order { get; set; }

        [XmlElement("Transaction", typeof(TransactionRequest))]
        public TransactionRequest Transaction { get; set; }
    }
}