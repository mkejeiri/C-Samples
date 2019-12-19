using Registration.Business.Exceptions;
using Registration.Business.Models;
using Registration.Business.Validators;

namespace Registration.Business.Handlers.UserValidation
{
    public class SocialSecurityNumberValidatorHandler : Handler<User>
    {
        private SocialSecurityNumberValidator socialSecurityNumberValidator
            = new SocialSecurityNumberValidator();

        public override void Handle(User request)
        {
            if (!socialSecurityNumberValidator.Validate(request.SocialSecurityNumber,
                request.CitizenshipRegion))
            {
                throw new UserValidationException("Social security number could not be validated");
            }
            base.Handle(request);
        }
    }
}
