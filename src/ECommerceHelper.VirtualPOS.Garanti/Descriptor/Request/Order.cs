using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    public class OrderRequest {

        public string OrderID { get; set; }
        public string GroupID { get; set; }
    }
}
