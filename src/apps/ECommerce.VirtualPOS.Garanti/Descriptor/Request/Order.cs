using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    internal class OrderRequest {

        public string OrderID { get; set; }
        public string GroupID { get; set; }
    }
}
