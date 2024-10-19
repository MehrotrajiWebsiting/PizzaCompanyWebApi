using System.ComponentModel.DataAnnotations;

namespace PizzaCompany.Models.Generated
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string UserEmail { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Phone { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
