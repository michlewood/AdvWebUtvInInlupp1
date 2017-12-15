using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kundregister.Entities
{
    public class MailConfiguration
    {
        public string MailServerHostName { get; set; }
        public bool SendMail { get; set; }

        public bool LogEverySentMail { get; set; }

        public string[] BlindCopyAddresses { get; set; }

    }
}
