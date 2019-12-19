using Payment_processing.Business.Models;
using Payment_processing.Business.PaymentProcessors;
using System.Linq;

namespace Payment_processing.Business.Handlers.PaymentHandlers
{
    public class InvoiceHandler : PaymentHandler
    {
        public InvoicePaymentProcessor InvoicePaymentProcessor { get; }
            = new InvoicePaymentProcessor();

        public override void Handle(Order order)
        {
            if (order.SelectedPayments.Any(x => x.PaymentProvider == PaymentProvider.Invoice))
            {
                InvoicePaymentProcessor.Finalize(order);
            }
            base.Handle(order);
        }
    }
}
