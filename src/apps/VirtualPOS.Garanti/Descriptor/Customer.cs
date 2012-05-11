using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor {

    [Serializable]
    public class Customer {

        [XmlElement("IPAddress")]
        public string IPAddress { get; set; }

        [XmlElement("EmailAddress")]
        public string EmailAddress { get; set; }
    }
}