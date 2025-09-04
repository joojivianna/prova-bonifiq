using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Payments;

namespace ProvaPub.Services
{
    public class OrderService
    {
        TestDbContext _ctx;

        private readonly IEnumerable<IPayment> _paymentMethods;

        public OrderService(TestDbContext ctx, IEnumerable<IPayment> paymentMethods)
        {
            _ctx = ctx;

            _paymentMethods = paymentMethods;
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
        {
            var method = _paymentMethods
                .FirstOrDefault(m => m.Name.Equals(paymentMethod, StringComparison.OrdinalIgnoreCase));

            if (method == null)
                throw new ArgumentException($"Método de pagamento '{paymentMethod}' não suportado.");

            await method.ProcessPayment(paymentValue, customerId);

            var order = new Order
            {
                CustomerId = customerId,
                Value = paymentValue,
                OrderDate = DateTime.UtcNow
            };

            return await InsertOrder(order);
        }

        private async Task<Order> InsertOrder(Order order)
        {
            await _ctx.Orders.AddAsync(order);
            await _ctx.SaveChangesAsync();

            TimeZoneInfo tz = TimeZoneInfo.CreateCustomTimeZone("UTC-3", TimeSpan.FromHours(-3), "UTC-3", "UTC-3");
            order.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, tz);

            return order;
        }
    }
}
