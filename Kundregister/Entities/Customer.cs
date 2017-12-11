
using System;
using System.ComponentModel.DataAnnotations;

namespace Kundregister.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        [CustomGender]
        public string Gender { get; set; }

        public int Age { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateEdited { get; set; }
    }
}