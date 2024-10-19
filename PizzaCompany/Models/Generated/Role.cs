using System.ComponentModel.DataAnnotations;

namespace PizzaCompany.Models.Generated
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
