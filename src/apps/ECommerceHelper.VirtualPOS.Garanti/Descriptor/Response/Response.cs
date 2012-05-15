using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.VirtualPOS.Garanti.Descriptor.Response {

    [Serializable]
    public class Response {

        [XmlIgnore]
        public PaymentResponseCode ResponseCode { 
            get {
                return (this.ReasonCode == "00") ? PaymentResponseCode.Successful : PaymentResponseCode.Unsuccessful;
            } 
        }

        public string Source { get; set; }
        public string Code { get; set; }
        public string ReasonCode { get; set; }
        public string Message { get; set; }
        public string ErrorMsg { get; set; }
        public string SysErrMsg { get; set; }
    }
}