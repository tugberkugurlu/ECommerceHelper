using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.CurrencyConverter.Descriptor {

    [Serializable]
    public class Currency {

        [XmlAttribute("CurrencyCode")]
        public string CurrencyCode { get; set; }

        [XmlElement("CurrencyName")]
        public string CurrencyName { get; set; }

        [XmlElement("Unit")]
        public string Unit { get; set; }

        [XmlElement("ForexBuying")]
        public string ForexBuying { get; set; }

        [XmlElement("ForexSelling")]
        public string ForexSelling { get; set; }

    }
}
