using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChameraVote.Utility
{
    class VoteClientValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(value is string)
            {
                return this.ValidateText(value as string);
            }
            return new ValidationResult(true,string.Empty);
        }

        public ValidationResult ValidateText(string value)
        {
            if (value is string)
            {
                var text = value as string;

                if (text.Contains(':'))
                {
                    return new ValidationResult(false, "':' is not allowed");
                }
                if (text.Contains('&'))
                {
                    return new ValidationResult(false, "'&' is not allowed");
                }
            }
            return new ValidationResult(true, string.Empty);
        }
    }
}
