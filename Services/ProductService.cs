using ProvaPub.Models;
using ProvaPub.Models.Base;
using ProvaPub.Repository;
using ProvaPub.Services.Base;

namespace ProvaPub.Services
{
	public class ProductService : BaseService<ProductList>
	{
        public ProductService(TestDbContext ctx) : base(ctx) { }

        public PageList<ProductList> ListProducts(int page) => ListPaged(page);
    }
}
