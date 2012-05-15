using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.CurrencyConverter.Descriptor {

    [Serializable]
    [XmlRoot("Tarih_Date")]
    public class RatesStatus {

        [XmlAttribute("Date")]
        public string Date { get; set; }

        [XmlElement("Currency", typeof(Currency))]
        public List<Currency> Currencies { get; set; }
    }
}
