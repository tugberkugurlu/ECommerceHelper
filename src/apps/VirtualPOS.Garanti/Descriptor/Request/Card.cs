using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    public class Card {

        public string Number { get; set; }
        public string ExpireDate { get; set; }
        public string CVV2 { get; set; }
    }
}
