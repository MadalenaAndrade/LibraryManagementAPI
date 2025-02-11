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
                if (value is string name)
                {
                    if (name.Contains(_defaultWord))
                        return false;
                }
                return true;
            }
        }
    }
}
