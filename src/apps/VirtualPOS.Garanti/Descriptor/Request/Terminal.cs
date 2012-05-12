using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    public class Terminal {

        [XmlElement("ProvUserID")]
        public string ProvUserID { get; set; }
        [XmlElement("HashData")]
        public string HashData { get; set; }
        [XmlElement("UserID")]
        public string UserID { get; set; }
        [XmlElement("ID")]
        public string ID { get; set; }
        [XmlElement("MerchantID")]
        public string MerchantID { get; set; }
    }
}
