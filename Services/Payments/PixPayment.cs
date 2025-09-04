namespace ProvaPub.Services.Payments
{
    public class PixPayment : IPayment
    {
        public string Name => "pix";

        public Task ProcessPayment(decimal value, int customerId)
        {
            return Task.CompletedTask;
        }
    }
}
