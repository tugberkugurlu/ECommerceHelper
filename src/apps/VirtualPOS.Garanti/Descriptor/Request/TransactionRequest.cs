using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Request {

    [Serializable]
    public class TransactionRequest {

        /// <summary>
        /// Defines the type of operation in digits in regards of CardholderPresentCode public property.
        /// If CardholderPresentCode is not set, it will fall back to 0 which represents the normal operation.
        /// </summary>
        [XmlElement("CardholderPresentCode")]
        public int CardholderPresentCodeDigit { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("InstallmentCnt")]
        public string InstallmentCnt { get; set; }
        
        [XmlElement("Amount")]
        public string AmountString { get; set; }

        [XmlElement("CurrencyCode")]
        public int CurrencyCodeDigit { get; set; }

        [XmlElement]
        public string MotoInd { 
            get {
                return "N";
            } 
        }
    }
}
