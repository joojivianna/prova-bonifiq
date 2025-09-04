using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Models.Base;
using ProvaPub.Repository;
using ProvaPub.Services.Base;
using ProvaPub.Services.Utils;

namespace ProvaPub.Services
{
    public class CustomerService : BaseService<CustomerList>
    {
        public PageList<CustomerList> ListCustomers(int page) => ListPaged(page);

        private readonly IDateTimeProvider _dateTimeProvider;
        
        public CustomerService(TestDbContext ctx) : base(ctx) { }

        public CustomerService(TestDbContext ctx, IDateTimeProvider dateTimeProvider) : base(ctx)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var baseDate = DateTime.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            if (_dateTimeProvider.UtcNow.Hour < 8 || _dateTimeProvider.UtcNow.Hour > 18 ||
                _dateTimeProvider.UtcNow.DayOfWeek == DayOfWeek.Saturday ||
                _dateTimeProvider.UtcNow.DayOfWeek == DayOfWeek.Sunday)
                return false;


            return true;
        }

    }
}
