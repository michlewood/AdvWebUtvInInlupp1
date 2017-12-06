using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kundregister.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string StreetName { get; set; }
        public int StreetNumber { get; set; }
        public int PostalCode { get; set; }
        public string Area { get; set; }
    }
}
