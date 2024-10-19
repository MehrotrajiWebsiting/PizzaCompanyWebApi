using System.ComponentModel.DataAnnotations;

namespace PizzaCompany.Models.Generated
{
    public class RoleChangeRequest
    {
        [Key]
        public int Id { get; set; } 
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RequestedRole { get; set; } = null!;
        public string Status { get; set; } = "PENDING";

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
