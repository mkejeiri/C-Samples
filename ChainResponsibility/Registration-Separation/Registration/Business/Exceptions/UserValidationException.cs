using System;

namespace Registration.Business.Exceptions
{
    public class UserValidationException : Exception
    {
        public UserValidationException(string message) : base(message)
        {

        }
    }
}
