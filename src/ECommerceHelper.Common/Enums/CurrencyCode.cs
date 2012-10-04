using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerceHelper.Common {

    /// <summary>
    /// Represents the currency codes defined in ISO 4217.
    /// </summary>
    public enum CurrencyCode {

        //According to ISO 4217
        //More info: http://en.wikipedia.org/wiki/ISO_4217

        /// <summary>
        /// Turkish lira
        /// </summary>
        TRY = 949,

        /// <summary>
        /// United States dollar
        /// </summary>
        USD = 840,

        /// <summary>
        /// Euro
        /// </summary>
        EUR = 978,

        /// <summary>
        /// Pound sterling
        /// </summary>
        GBP = 826,

        /// <summary>
        /// Japanese yen
        /// </summary>
        JPY = 392,

        /// <summary>
        /// Canadian dollar
        /// </summary>
        CAD = 124,

        /// <summary>
        /// Danish krone
        /// </summary>
        DKK = 208
    }
}