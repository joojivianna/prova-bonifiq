namespace ProvaPub.Services.Payments
{
    public class CreditCardPayment : IPayment
    {
        public string Name => "creditcard";

        public Task ProcessPayment(decimal value, int customerId)
        {
            return Task.CompletedTask;
        }
    }
}
