using System.ComponentModel.DataAnnotations;

namespace PizzaCompany.Models.Generated
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public decimal? Price { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
