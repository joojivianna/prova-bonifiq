using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Models.Base;
using ProvaPub.Repository;

namespace ProvaPub.Services.Base
{
    public abstract class BaseService<T> where T : class
    {
        protected readonly TestDbContext _ctx;
        protected BaseService(TestDbContext ctx) 
        {
            _ctx = ctx;
        }

        public PageList<T> ListPaged(int page)
        {
            const int pageSize = 10;

            var items = _ctx.Set<T>()
                .OrderBy(e => EF.Property<int>(e, "Id"))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = _ctx.Customers.Count();
            var hasNext = page * pageSize < totalCount;

            return new PageList<T>
            {
                Items = items,
                TotalCount = totalCount,
                HasNext = hasNext
            };
        }
    }
}
