using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.VirtualPOS.Garanti.Descriptor.Response {

    [Serializable]
    public class Response {

        [XmlIgnore]
        public PaymentResponseCode ResponseCode { 
            get {
                return (this.ReasonCode == "00") ? PaymentResponseCode.Successful : PaymentResponseCode.Unsuccessful;
            } 
        }

        [XmlElement("Source")]
        public string Source { get; set; }
        
        [XmlElement("Code")]
        public string Code { get; set; }
        
        [XmlElement("ReasonCode")]
        public string ReasonCode { get; set; }
        
        [XmlElement("Message")]
        public string Message { get; set; }
        
        [XmlElement("ErrorMsg")]
        public string ErrorMsg { get; set; }

        [XmlElement("SysErrMsg")]
        public string SysErrMsg { get; set; }

    }
}
