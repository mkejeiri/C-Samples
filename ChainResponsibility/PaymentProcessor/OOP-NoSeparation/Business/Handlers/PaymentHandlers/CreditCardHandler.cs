using Payment_processing.Business.Models;
using Payment_processing.Business.PaymentProcessors;
using System.Linq;

namespace Payment_processing.Business.Handlers.PaymentHandlers
{
    public class CreditCardHandler : PaymentHandler
    {
        public CreditCardPaymentProcessor CreditCardPaymentProcessor { get; }
            = new CreditCardPaymentProcessor();

        public override void Handle(Order order)
        {
            if (order.SelectedPayments.Any(x => x.PaymentProvider == PaymentProvider.CreditCard))
            {
                CreditCardPaymentProcessor.Finalize(order);
            }

            base.Handle(order);
        }
    }
}
