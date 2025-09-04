using ProvaPub.Models.Base;

namespace ProvaPub.Models
{
	public class ProductList : PageList<Product>
	{
		public List<Product> Products { get; set; }
	}
}
