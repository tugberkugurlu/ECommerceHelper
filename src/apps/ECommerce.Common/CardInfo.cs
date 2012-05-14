using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Common {

    public class CardInfo {

        private static string getXMLContent() {

            using (Stream stream = typeof(CardInfo).Assembly.GetManifestResourceStream(string.Format("ECommerce.Common.Resources.{0}", "CardsInfo.xml")))
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

        private static string extractBINCode(string creditCardNumber) {

            return creditCardNumber.Substring(0, 6);
        }

        public static bool IsCreditCardTurkish(string creditCardNumber) {

            var cards = getCards();
            var creditCardBINNo = extractBINCode(creditCardNumber);

            return (cards.Any(card => card.BINNo == creditCardBINNo)) ? true : false;
        }
    }
}