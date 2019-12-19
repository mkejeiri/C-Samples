using System;
using Registration.Business.Exceptions;
using Registration.Business.Models;

namespace Registration.Business.Handlers.UserValidation
{
    public class AgeValidationHandler : IReceiver<User>
    {
        public void Handle(User user)
        {
            if (user.Age < 18)
            {
                throw new UserValidationException("You have to be 18 years or older");
            }

            //base.Handle(user);
        }
    }
}
