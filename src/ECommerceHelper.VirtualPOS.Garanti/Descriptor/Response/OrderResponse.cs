using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.VirtualPOS.Garanti.Descriptor.Response {

    [Serializable]
    public class OrderResponse {

        public string OrderID { get; set; }
    }
}
