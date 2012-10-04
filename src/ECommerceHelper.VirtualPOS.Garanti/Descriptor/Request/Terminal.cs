using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    public class Terminal {

        public string ID { get; set; }
        public string ProvUserID { get; set; }
        public string HashData { get; set; }
        public string UserID { get; set; }
        public string MerchantID { get; set; }
    }
}
