using ProvaPub.Models.Base;

namespace ProvaPub.Models
{
	public class CustomerList : PageList<Customer>
	{
		public List<Customer> Customers { get; set; }
	}
}
