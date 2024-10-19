namespace PizzaCompany.Models.Generated
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; } = null!; // e.g., "Orders"
        public string Action { get; set; } = null!; // e.g., "Added", "Modified", "Deleted"
        public string KeyValues { get; set; } = null!; // e.g., "OrderId=123"
        public string OldValues { get; set; } = null!; // JSON string of old values (for updates and deletes)
        public string NewValues { get; set; } = null!; // JSON string of new values (for inserts and updates)
        public string UserId { get; set; } = null!; // The user who made the change
        public DateTime Timestamp { get; set; } // Time of change
    }
}
