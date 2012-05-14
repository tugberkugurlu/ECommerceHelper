using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.VirtualPOS.Garanti {

    internal enum BankResponseCode {

        //NOTE: Experimental for now

        OK = 00,
        ActionUnapproved = 05,
        RequestDeclined = 06,
        InvalidAction = 12,
        InvalidAmount = 13,
        CreditCardNumberIncorrect = 14,
        IEMRoutingProblem = 15,
        InadequateBalance = 16,
        ActionCancelled = 17,
        CreditCardClosed = 18,
        CreditCardAcountNotAvailable = 51
    }
}