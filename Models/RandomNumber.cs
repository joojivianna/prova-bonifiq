using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProvaPub.Models
{
    [Index(nameof(Number), IsUnique = true)]
    public class RandomNumber
    {
        public int Id { get; set; }
        public int Number { get; set; }
    }
}
