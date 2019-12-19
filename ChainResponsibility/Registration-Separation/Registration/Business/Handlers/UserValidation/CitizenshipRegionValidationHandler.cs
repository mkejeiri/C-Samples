using System;
using Registration.Business.Exceptions;
using Registration.Business.Models;

namespace Registration.Business.Handlers.UserValidation
{
    public class CitizenshipRegionValidationHandler : IReceiver<User>
    {
        public void Handle(User user)
        {
            if (user.CitizenshipRegion.TwoLetterISORegionName == "NO")
            {
                throw new UserValidationException("We currently do not support Norwegians");
            }

            //base.Handle(user);
        }
    }
}
