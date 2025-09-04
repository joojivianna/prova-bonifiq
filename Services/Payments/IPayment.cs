namespace ProvaPub.Services.Payments
{
    public interface IPayment
    {
        string Name { get; }
        Task ProcessPayment(decimal value, int customerId);
    }
}
