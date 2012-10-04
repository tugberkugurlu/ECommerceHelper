using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceHelper.Common;
using ECommerceHelper.CurrencyConverter.Descriptor;

namespace ECommerceHelper.CurrencyConverter {

    public interface ICurrencyService {

        Task<ConvertResponseContext> ConvertAsync(decimal amount, CurrencyCode fromCurrency);
        Task<ConvertResponseContext> ConvertAsync(decimal amount, CurrencyCode fromCurrency, CurrencyCode toCurrency);
    }
}