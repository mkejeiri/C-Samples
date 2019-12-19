using Registration.Business.Exceptions;
using Registration.Business.Handlers.UserValidation;
using Registration.Business.Models;

namespace Registration.Business
{
    public class UserProcessor
    {
        public bool Register(User user)
        {
            try
            {
                var handler = new SocialSecurityNumberValidatorHandler();

                handler.SetNext(new AgeValidationHandler())
                    .SetNext(new NameValidationHandler())
                    .SetNext(new CitizenshipRegionValidationHandler());

                handler.Handle(user);
            }
            catch (UserValidationException ex)
            {
                return false;
            }

            return true;
        }
    }
}
