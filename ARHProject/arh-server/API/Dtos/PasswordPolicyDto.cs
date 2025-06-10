using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class PasswordPolicyDto
    {
        public int MinPasswordLength { get; set; }
        public int MaxPasswordLength { get; set; }
        public bool DigitRequired { get; set; }
        public bool UppercaseLetterRequired { get; set; }
        public bool LowercaseLetterRequired { get; set; }
        public bool SpecialCharacterRequired { get; set; }
        public int PasswordAgeInDays { get; set; }
        public string Password { get; set; }

    }
}