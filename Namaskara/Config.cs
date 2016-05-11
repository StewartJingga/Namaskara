using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Namaskara.Models
{
    public class Config
    {
        public static int DaysToConfirm = 2; //Day before the submitted order becomes expired.

        public static string WishlistFailed = "You need to login to access this feature.";

        public static string WishlistSuccess = "Item has been successfully added to your wishlist.";

        public static string WishlistExists = "Item is already in the wishlist.";

        public static string[] StatusList = { "","Order Submitted",
                                     "Waiting For Confirmation",
                                    "Confirmed", "Completed", "Postponed", "Expired"};

        public static string[] UnitList = { "kg", "g", "ltr", "ml" };

        public static string SendingEmailConfirmationMessage =
            String.Format("<h2>Thank you for purchasing at our store!</h2><br>Your order has been submitted.<br>Please review your order below.<br>"
                            +"");

        public static string[] BankAccount = { "BCA", "Ni Komang Ayu Novita Sari", "7730379573" };//Bank, Account Name, Account Number
        public static string WhatsAppNumber = "+62 822 1155 5753";

        public static string SmtpHost = "mail.namaskarasuperfood.com";
        public static int SmtpPort = 25;
        public static string Email = "admin@namaskarasuperfood.com";
        public static string Password = "password";

        //public static string SmtpHost = "mail.mandiri-jaya.com";
        //public static int SmtpPort = 25;
        //public static string Email = "stewart@mandiri-jaya.com";
        //public static string Password = "smithsog00d";

        //public static string SmtpHost = "mail.namaskarasuperfood.com";
        //public static int SmtpPort = 25;
        //public static string Email = "admin2@namaskarasuperfood.com";
        //public static string Password = "elsacintaoscar";

        public static Dictionary<Enum, int> PackagingInGrams = new Dictionary<Enum, int>
        {
            [PackagingEnum.Bottle] = 500,
            [PackagingEnum.Carton] = 200,
            [PackagingEnum.Nothing] = 0
        };
    }
}