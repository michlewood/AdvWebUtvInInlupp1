using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kundregister.Entities
{
    public class CustomGenderAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            string genderValue = value.ToString();

            if (genderValue == "transgender" || genderValue == "female" || genderValue == "other" || genderValue == "male")
                return true;

            else return false;
        }
    }
}
