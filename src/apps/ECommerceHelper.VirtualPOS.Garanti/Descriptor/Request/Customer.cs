using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    public class Customer {

        public string IPAddress { get; set; }
        public string EmailAddress { get; set; }
    }
}