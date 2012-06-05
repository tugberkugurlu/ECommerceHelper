using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerceHelper.Common {

    public class CardInfo {

        private static readonly int[] amexCardPrefixes = new[] { 34, 37 };
        private static readonly int[] visaCardPrefixes = new[] { 4 };
        private static readonly int[] mastercardCardPrefixes = new[] { 51, 55 };
        private static readonly int[] discoverCardPrefixes = new[] { 6011, 644, 65 };

        private static List<Card> _cards = getCards();

        /// <summary>
        /// Determines if the specified credit card is a card issued by a Turkish bank.
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        public static bool IsCreditCardTurkish(string creditCardNumber) {

            var creditCardBINNo = extractBINCode(creditCardNumber);
            return (_cards.Any(card => card.BINNo == creditCardBINNo)) ? true : false;
        }

        /// <summary>
        /// Determines if the specified credit card is an American Express credit card
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        public static bool IsCreditCardAMEX(string creditCardNumber) {

            return isSpecificCreditCard(creditCardNumber, amexCardPrefixes);
        }

        /// <summary>
        /// Determines if the specified credit card is an Visa credit card
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        public static bool IsCreditCardVISA(string creditCardNumber) {

            return isSpecificCreditCard(creditCardNumber, visaCardPrefixes);
        }

        /// <summary>
        /// Determines if the specified credit card is an MasterCard credit card
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        public static bool IsCreditCardMasterCard(string creditCardNumber) {

            return isSpecificCreditCard(creditCardNumber, mastercardCardPrefixes);
        }

        //private helpers

        private static string getXMLContent() {

            using (Stream stream = typeof(CardInfo).Assembly.GetManifestResourceStream(string.Format("ECommerceHelper.Common.Resources.{0}", "CardsInfo.xml")))
            using (StreamReader streamReader = new StreamReader(stream))
                return streamReader.ReadToEnd();
        }

        private static List<Card> deseserializeCards() {

            XmlSerializer serializer = new XmlSerializer(typeof(List<Card>));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(getXMLContent())))
                return (List<Card>)serializer.Deserialize(ms);
        }

        private static List<Card> getCards() {

            return deseserializeCards();
        }

        /// <summary>
        /// The fist 6 digits are the Issuer Identification Number. It will identify the institution that issued the card.
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <returns></returns>
        private static string extractBINCode(string creditCardNumber) {

            return creditCardNumber.Substring(0, 6);
        }

        private static bool isSpecificCreditCard(string creditCardNumber, int[] prefixes) {

            var creditCardBINNo = extractBINCode(creditCardNumber);

            foreach (var prefix in prefixes) {
                if (creditCardBINNo.StartsWith(prefix.ToString())) {
                    return true;
                }
            }

            return false;
        }
    }
}