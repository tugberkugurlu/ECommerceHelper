using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VirtualPOS.Garanti {

    /// <summary>
    /// Utility class for this VirtualPOS project
    /// </summary>
    internal class PaymentUtility {

        /// <summary>
        /// Special encryption utility method for Garanti Bank Virtual POS Credentials
        /// </summary>
        /// <param name="terminal_id">Terminal ID [Başına 0 eklenerek 9 digite tamamlanmalıdır.]</param>
        /// <param name="terminal_userid_password">Terminal UserID şifresi</param>
        /// <returns></returns>
        public static string EncryptVirtualPOSCredentials(string terminal_id, string terminal_userid_password) {

            //if the terminal id Lenght is less than 9 digit
            if (terminal_id.Length != 9) {

                var leftoverLenght = 9 - terminal_id.Length;
                for (int i = 0; i < leftoverLenght; i++)
                    terminal_id = String.Format("0{0}", terminal_id);
            }

            string encryptedValue = String.Format("{0}{1}", terminal_userid_password, terminal_id);

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