using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Response {

    [Serializable]
    public class OrderResponse {

        [XmlElement("OrderID")]
        public string OrderID { get; set; }
    }
}
