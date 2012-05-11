using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Common {

    /// <summary>
    /// Represents the currency codes defined in ISO 4217.
    /// </summary>
    public enum CurrencyCode {

        //According to ISO 4217
        //More info: http://en.wikipedia.org/wiki/ISO_4217

        TRY = 949,
        USD = 840,
        EUR = 978,
        GBP = 826,
        JPY = 392
    }
}