using System;
using System.Collections.Generic;
using Registration.Business.Models;

namespace Registration.Business.Handlers
{
    public class UserValidationHandler
    {
        private readonly IList<IReceiver<User>> receivers;

        public UserValidationHandler(params IReceiver<User>[] receivers)
        {
            this.receivers = receivers;
        }

        public void Handle(User user)
        {
            //This loop play the role of next
            foreach (var receiver in receivers)
            {
                Console.WriteLine($"Running: {receiver.GetType().Name}");
                receiver.Handle(user);
            }

        }

        public void SetNext(IReceiver<User> next)
        {
            receivers.Add(next);
        }
    }

}
