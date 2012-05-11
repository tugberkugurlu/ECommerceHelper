using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor {

    [Serializable]
    public class Order {

        [XmlElement("OrderID")]
        public string OrderID { get; set; }

        [XmlElement("GroupID")]
        public string GroupID { get; set; }
    }
}
