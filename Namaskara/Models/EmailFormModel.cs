using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Namaskara.Models
{
    public class EmailFormModel
    {
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Destination { get; set; }
        public AlternateView Message { get; set; }

    }
}