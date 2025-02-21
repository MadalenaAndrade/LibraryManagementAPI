using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LibraryManagementAPI.DTOs
{
    public class CustomAttributes
    {
        public class DefaultNameNotAllowedAttribute : ValidationAttribute
        {
            private readonly string _defaultWord;
            public DefaultNameNotAllowedAttribute(string defaultWord)
            {
                _defaultWord = defaultWord;
            }

            public override bool IsValid(object value)
            {

                string name = (string)value;

                if (name.Contains(_defaultWord))
                        return false;
                
                return true;
            }
        }

        public class NumberValidationAttribute : ValidationAttribute
        {
            private readonly long _minValue;
            private readonly int? _requiredLength;

            // There is two constructors as I wanted my attribute to have two overload

            // 1st: to validates only the minimum value allowed
            public NumberValidationAttribute(long minValue)
            {
                _minValue = minValue;
                _requiredLength = null;
            }

            // 2nd: To validate not only the minimum value but also required length
            public NumberValidationAttribute(long minValue, int requiredLength)
            {
                _minValue = minValue;
                _requiredLength = requiredLength;
            }

            public override bool IsValid(object value)
            {
                if (value == null)
                    return true;

                // Tries to convert the object to "single-precision floating-point number" to allow the attribute compatibility with other datatypes,
                // ToDo apply method to consider how the values are rounded by default
                var number = Convert.ToInt64(value);

                // Validates minimum value
                if (number < _minValue)
                    return false;

                // Validates length if a number has been provided
                if (_requiredLength.HasValue)
                {
                    if (number.ToString().Length != _requiredLength.Value)
                        return false;
                }

                return true;
            }
        }

        public class YearValidationAttribute : ValidationAttribute
        {
            private readonly short _minYear;

            public YearValidationAttribute(short minYear)
            {
                _minYear = minYear;
            }

            public override bool IsValid(object value)
            {
                if (value == null) 
                    return true;

                short year = (short)value;
                short maxYear = (short)DateTime.Now.Year;

                if (year < _minYear || year > maxYear)
                    return false;

                return true;
            }
        }

        public class DateValidationAttribute : ValidationAttribute
        {
            private readonly string[] _requiredFormats;

            public DateValidationAttribute(string requiredFormats)
            {
                _requiredFormats = requiredFormats.Split("||");
            }

            public override bool IsValid(object value)
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    return true;

                string dateString = value.ToString();

                if (string.IsNullOrWhiteSpace(dateString))
                    return true;

                foreach (var format in _requiredFormats)
                {
                    if (DateTime.TryParseExact(dateString, format.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var tempResult))
                    {
                        return true;
                    }
                        
                }

                return false;
            }
        }

        public class NIFValidationAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    return false;

                string nifNumber = value.ToString();

                return ValidateNIF(nifNumber);
            }

            private bool ValidateNIF(string nifNumber)
            {
                int numberLength = 9; //NIF has always 9 numbers

                string filteredNumber = Regex.Match(nifNumber, @"[0-9]+").Value; // extract number

                if (filteredNumber.Length != numberLength || int.Parse(filteredNumber[0].ToString()) == 0)
                    return false;

                int checkSum = 0;

                for (int i = 0; i < numberLength -1; i++)
                {
                    checkSum += (int.Parse(filteredNumber[i].ToString())) * (numberLength - i);
                }

                int verifiedDigit = 11 - (checkSum % 11);

                if (verifiedDigit > 9) 
                {
                    verifiedDigit = 0;
                }

                return verifiedDigit == int.Parse(filteredNumber[numberLength - 1].ToString());

            }
        }
    }
}
