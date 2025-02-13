using System.ComponentModel.DataAnnotations;

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
                short year = (short)value;
                short maxYear = (short)DateTime.Now.Year;

                if (year < _minYear || year > maxYear)
                    return false;

                return true;
            }
        }
    }
}
