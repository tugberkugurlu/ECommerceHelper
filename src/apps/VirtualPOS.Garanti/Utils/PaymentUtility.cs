using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.VirtualPOS.Garanti {

    /// <summary>
    /// Utility class for this VirtualPOS project
    /// </summary>
    public class PaymentUtility {

        public static string EncodeExpireDate(int month, int year) {

            if (year.ToString().Length != 4)
                throw new ArgumentException("The length of the year is not 4.", "year");

            var specialYear = year.ToString().Substring(2);
            var speacialMonth = (month.ToString().Length == 1) ? string.Format("0{0}", month.ToString()) : month.ToString();

            return string.Format(
                "{0}{1}", speacialMonth, specialYear
            );
        }

        public static string EncodeDecimalPaymentAmount(decimal amount) {

            return amount.ToString("0.00").TrimStart('0').Replace(",", "").Replace(".", "");
        }

        public static string GenerateHASHedData(
            string terminalId, string creditCardNumber, string properlyEncodedPaymentAmount, 
            string terminalPassword) {
            
            return getSHA1(
                string.Format("{0}{1}{2}{3}{4}", 
                    null, terminalId, creditCardNumber, properlyEncodedPaymentAmount,
                    EncryptVirtualPOSCredentials(terminalId, terminalPassword)
                )
            ).ToUpper();
        }

        /// <summary>
        /// Special encryption utility method for Garanti Bank Virtual POS Credentials
        /// </summary>
        /// <param name="terminalId">Terminal ID [Başına 0 eklenerek 9 digite tamamlanmalıdır.]</param>
        /// <param name="terminalUserIdPassword">Terminal UserID şifresi</param>
        /// <returns></returns>
        private static string EncryptVirtualPOSCredentials(string terminalId, string terminalUserIdPassword) {

            if (string.IsNullOrEmpty(terminalId))
                throw new ArgumentNullException("terminalId");

            if (string.IsNullOrEmpty(terminalUserIdPassword))
                throw new ArgumentNullException("terminalUserIdPassword");

            //if the terminal id Lenght is less than 9 digit
            if (terminalId.Length != 9) {

                var leftoverLenght = 9 - terminalId.Length;
                for (int i = 0; i < leftoverLenght; i++)
                    terminalId = String.Format("0{0}", terminalId);
            }

            string encryptedValue = String.Format("{0}{1}", terminalUserIdPassword, terminalId);

            return getSHA1(encryptedValue).ToUpper();
        }

        //private helpers
        private static string getHexaDecimal(byte[] bytes) {

            StringBuilder strBuilder = new StringBuilder();

            for (int n = 0; n < bytes.Length; n++)
                strBuilder.Append(string.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));

            return strBuilder.ToString();
        }
        private static string getSHA1(string SHA1Data) {

            using (SHA1 sha = new SHA1CryptoServiceProvider()) {

                string hashedPassword = SHA1Data;
                byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(hashedPassword);
                byte[] inputBytes = sha.ComputeHash(hashbytes);

                return getHexaDecimal(inputBytes);
            }
        }

    }
}