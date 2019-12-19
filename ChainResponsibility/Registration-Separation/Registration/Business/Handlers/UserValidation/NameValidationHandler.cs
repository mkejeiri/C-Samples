
using System;
using System.Threading.Channels;
using Registration.Business.Exceptions;
using Registration.Business.Models;

namespace Registration.Business.Handlers.UserValidation
{
    public class NameValidationHandler : IReceiver<User>
    {
        public void Handle(User user)
        {
            if (user.Name.Length <= 1)
            {
                throw new UserValidationException("Your name is unlikely this short.");
            }

            //base.Handle(user);
        }
    }
}
