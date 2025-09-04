namespace ProvaPub.Services.Payments
{
    public class PaypalPayment : IPayment
    {
        public string Name => "paypal";

        public Task ProcessPayment(decimal value, int customerId)
        {
            return Task.CompletedTask;
        }
    }
}
