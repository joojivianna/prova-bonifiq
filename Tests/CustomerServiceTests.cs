using Microsoft.EntityFrameworkCore;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Services.Utils;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private TestDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TestDbContext(options);
        }

        [Fact]
        public async Task Throws_WhenCustomerIdIsInvalid()
        {
            var ctx = CreateContext("test1");
            var dateProvider = new Mock<IDateTimeProvider>();
            var service = new CustomerService(ctx, dateProvider.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(0, 50));
        }

        [Fact]
        public async Task Throws_WhenPurchaseValueIsInvalid()
        {
            var ctx = CreateContext("test2");
            var dateProvider = new Mock<IDateTimeProvider>();
            var service = new CustomerService(ctx, dateProvider.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CanPurchase(1, 0));
        }

        [Fact]
        public async Task Throws_WhenCustomerIdNotExist()
        {
            var ctx = CreateContext(Guid.NewGuid().ToString());
            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(x => x.UtcNow).Returns(new DateTime(2023, 9, 4, 10, 0, 0, DateTimeKind.Utc));

            var service = new CustomerService(ctx, dateProvider.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CanPurchase(100, 50));
        }

        [Fact]
        public async Task ReturnsFalse_WhenCustomerHasOrderInLastMonth()
        {
            var ctx = CreateContext("test4");
            ctx.Customers.Add(new Customer { Id = 1, Name = "test1" });
            ctx.Orders.Add(new Order { CustomerId = 1, OrderDate = DateTime.UtcNow });
            ctx.SaveChanges();

            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            var service = new CustomerService(ctx, dateProvider.Object);
            var result = await service.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Fact]
        public async Task ReturnsFalse_WhenFirstPurchaseOver100()
        {
            var ctx = CreateContext("test5");
            ctx.Customers.Add(new Customer { Id = 1, Name = "test2" });
            ctx.SaveChanges();

            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(x => x.UtcNow).Returns(new DateTime(2025, 1, 15, 10, 0, 0));

            var service = new CustomerService(ctx, dateProvider.Object);
            var result = await service.CanPurchase(1, 150);

            Assert.False(result);
        }

        [Fact]
        public async Task ReturnsFalse_WhenOutsideBusinessHours()
        {
            var ctx = CreateContext("test6");
            ctx.Customers.Add(new Customer { Id = 1, Name = "test3" });
            ctx.SaveChanges();

            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(x => x.UtcNow).Returns(new DateTime(2025, 1, 15, 22, 0, 0));

            var service = new CustomerService(ctx, dateProvider.Object);
            var result = await service.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Fact]
        public async Task ReturnsTrue_WhenAllRulesPass()
        {
            var ctx = CreateContext("test7");
            ctx.Customers.Add(new Customer { Id = 1, Name = "test4" });
            ctx.SaveChanges();

            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(x => x.UtcNow).Returns(new DateTime(2025, 1, 15, 10, 0, 0)); // dia útil, 10h

            var service = new CustomerService(ctx, dateProvider.Object);
            var result = await service.CanPurchase(1, 50);

            Assert.True(result);
        }
    }
}
