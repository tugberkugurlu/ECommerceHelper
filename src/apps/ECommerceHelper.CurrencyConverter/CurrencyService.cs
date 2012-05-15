using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;
using ECommerceHelper.Common;
using ECommerceHelper.CurrencyConverter.Descriptor;

namespace ECommerceHelper.CurrencyConverter {

    public class CurrencyService : ICurrencyService {

        private HttpContext _currentHttpContext;

        public async Task<ConvertResponseContext> ConvertAsync(decimal amount, CurrencyCode fromCurrency) {

            return await ConvertAsync(amount, fromCurrency, CurrencyCode.TRY);
        }

        public async Task<ConvertResponseContext> ConvertAsync(decimal amount, CurrencyCode fromCurrency, Common.CurrencyCode toCurrency) {

            if (!Enum.IsDefined(typeof(CurrencyCode), fromCurrency))
                throw new ArgumentNullException("fromCurrency");

            if (!Enum.IsDefined(typeof(CurrencyCode), toCurrency))
                throw new ArgumentNullException("toCurrency");

            if (fromCurrency == toCurrency)
                throw new ArgumentException("fromCurrency and toCurrency paramaters cannot be the same");

            //NOTE: HttpContext.Current nulls itself after async call
            //get a reference of it and work on that one.
            _currentHttpContext = HttpContext.Current;

            var ratesStatus = await getRatesStatusAsync();

            var fromCurrencyStr = fromCurrency.ToString();
            var toCurrencyStr = toCurrency.ToString();

            //NOTE: we are here sure that these two paramates have not the same value set
            if (fromCurrency == CurrencyCode.TRY || toCurrency == CurrencyCode.TRY) {

                //NOTE: request has been made for some sort of TRY convert
                //deal with it accordingly

                //NOTE: if request is to convert from TRY
                if (fromCurrency == CurrencyCode.TRY) {

                    var ___currency = ratesStatus.Currencies.FirstOrDefault(cur =>
                        cur.CurrencyCode == toCurrencyStr && !string.IsNullOrEmpty(cur.ForexSelling)
                    );

                    if (___currency == null) {

                        return new ConvertResponseContext {
                            IsConvertSuccessful = false,
                            BaseAmount = amount,
                            ErrorMessage = string.Format(
                                "There is no currency rate available for {0} currency", toCurrencyStr
                            )
                        };
                    }

                    decimal ___currenyRate;
                    if (!decimal.TryParse(___currency.ForexSelling, out ___currenyRate)) {

                        return new ConvertResponseContext {
                            IsConvertSuccessful = false,
                            BaseAmount = amount,
                            ErrorMessage = string.Format(
                                "{0} amount for {1} currency is not in a correct format.", "ForexSelling", toCurrencyStr
                            )
                        };
                    }

                    var ___convertedAmount = (amount / ___currenyRate);
                    
                    return new ConvertResponseContext {
                        IsConvertSuccessful = true,
                        BaseAmount = amount,
                        BaseCurrencyRate = ___currenyRate,
                        ConvertedFrom = fromCurrency,
                        ConvertedTo = toCurrency,
                        ConvertedAmount = decimal.Round(___convertedAmount, 2)
                    };
                }
                //NOTE: if request is to convert to TRY
                else {

                    var ___currency = ratesStatus.Currencies.FirstOrDefault(cur =>
                        cur.CurrencyCode == fromCurrencyStr && !string.IsNullOrEmpty(cur.ForexSelling)
                    );

                    if (___currency == null) {

                        return new ConvertResponseContext {
                            IsConvertSuccessful = false,
                            BaseAmount = amount,
                            ErrorMessage = string.Format(
                                "There is no currency rate available for {0} currency", fromCurrencyStr
                            )
                        };
                    }

                    decimal ___currenyRate;
                    if (!decimal.TryParse(___currency.ForexSelling, out ___currenyRate)) {

                        return new ConvertResponseContext {
                            IsConvertSuccessful = false,
                            BaseAmount = amount,
                            ErrorMessage = string.Format(
                                "{0} amount for {1} currency is not in a correct format.", "ForexSelling", fromCurrencyStr
                            )
                        };
                    }

                    var ___convertedAmount = (amount * ___currenyRate);

                    return new ConvertResponseContext {
                        IsConvertSuccessful = true,
                        BaseAmount = amount,
                        BaseCurrencyRate = ___currenyRate,
                        ConvertedFrom = fromCurrency,
                        ConvertedTo = toCurrency,
                        ConvertedAmount = decimal.Round(___convertedAmount, 2)
                    };
                }
            }
            else {

                //NOTE: request has been made for some sort of cross rate calculation
                //deal with it accordingly

                return new ConvertResponseContext {
                    IsConvertSuccessful = false,
                    BaseAmount = amount,
                    ErrorMessage = string.Format(
                        "Cross-rate calculation is not supported."
                    )
                };
            }
        }

        //private helpers
        private async Task<RatesStatus> getRatesStatusAsync() {

            //NOTE: See if HttpContext.Current is null or not first.
            if (_currentHttpContext != null) {

                var cachedRatesStatusObj = _currentHttpContext.Cache[Constants.RATESSTATUS_CACHE_KEY_NAME];
                if (cachedRatesStatusObj != null)
                    return (RatesStatus)cachedRatesStatusObj;
            }

            using (HttpClient httpClient = new HttpClient()) {

                var responseString = await httpClient.GetStringAsync(
                    Constants.BASE_CURRENCY_RATES_REQUEST_ADDRESS
                );

                XmlSerializer serializer = new XmlSerializer(typeof(RatesStatus));
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString))) {

                    var ratesStatus = (RatesStatus)serializer.Deserialize(ms);

                    //NOTE: try to cache the object before returning
                    cacheRatesStatusObject(ratesStatus);

                    return ratesStatus;
                }
            }
        }

        private void cacheRatesStatusObject(RatesStatus ratesStatus) {

            if (ratesStatus == null)
                throw new ArgumentNullException("ratesStatus");

            //NOTE: See if HttpContext.Current is null or not first.
            if (_currentHttpContext == null)
                return;

            var ratesStatusDate = DateTime.ParseExact(
                ratesStatus.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture
            );

            //NOTE: According to TCMB, the currency rates are changed after 15:30
            //in every business day. We are setting this accordingly.
            var cacheAbsoluteExpirationDate = ratesStatusDate.AddDays(1).AddHours(15).AddMinutes(31);

            //NOTE: If the date when the xml doc has been created is not a business date
            //then, cacheAbsoluteExpirationDate might set to a date which is before now.
            //Deal with this accordingly.
            var todayAbsoluteExpirationDate = DateTime.Today.AddHours(15).AddMinutes(31);
            if(cacheAbsoluteExpirationDate <= todayAbsoluteExpirationDate)
                cacheAbsoluteExpirationDate = todayAbsoluteExpirationDate.AddDays(1);

            //Add the cache properly
            _currentHttpContext.Cache.Add(
                Constants.RATESSTATUS_CACHE_KEY_NAME,
                ratesStatus,
                null,
                cacheAbsoluteExpirationDate,
                Cache.NoSlidingExpiration, 
                CacheItemPriority.High,
                (x, y, z) => { }
            );
        }
    }
}