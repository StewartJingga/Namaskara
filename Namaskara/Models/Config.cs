using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class Config
    {
        public static int DaysToConfirm = 2; //Day before the submitted order becomes expired.

        public static string[] StatusList = { "","Order Submitted",
                                     "Waiting For Confirmation",
                                    "Confirmed", "Completed", "Postponed", "Expired"};

        public static string SendingEmailConfirmationMessage =
            String.Format("<h2>Thank you for purchasing at our store!</h2><br>Your order has been submitted.<br>Please review your order below.<br>"
                            +"");
    }
}