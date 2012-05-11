using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Common;
using ECommerce.CurrencyConverter.Descriptor;

namespace ECommerce.CurrencyConverter {

    public interface ICurrencyService : IDisposable {

        Task<ConvertResponseContext> ConvertAsync(decimal amount, CurrencyCode fromCurrency);
        Task<ConvertResponseContext> ConvertAsync(decimal amount, CurrencyCode fromCurrency, CurrencyCode toCurrency);
        Task<RatesStatus> GetRatesStatusAsync();
    }
}