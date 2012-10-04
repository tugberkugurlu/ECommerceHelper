using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerceHelper.CurrencyConverter {

    internal class Constants {

        internal const string BASE_CURRENCY_RATES_REQUEST_ADDRESS = "http://www.tcmb.gov.tr/kurlar/today.xml";
        internal const string RATESSTATUS_CACHE_KEY_NAME = "___ECommerce.CurrencyConverter_RATESSTATUS_CACHE";
    }
}
