using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerceHelper.Common {

    [Serializable]
    public class Card {

        public string BINNo { get; set; }
        public string Organization { get; set; }
        public string BankCode { get; set; }
        public string Debit { get; set; }
        public string ProductClass { get; set; }
        public string Status { get; set; }
        public string Fraud { get; set; }
    }
}