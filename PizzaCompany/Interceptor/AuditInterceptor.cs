using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using PizzaCompany.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Synchronous SavingChanges method
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        var userId = GetUserId();

        // Collect audit entries in a separate list
        var auditEntries = CollectAuditEntries(context, userId);

        // Add audit entries after collection is complete
        context.Set<AuditLog>().AddRange(auditEntries);

        return base.SavingChanges(eventData, result);
    }

    // Asynchronous SavingChangesAsync method
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var userId = GetUserId();

        // Collect audit entries in a separate list
        var auditEntries = CollectAuditEntries(context, userId);

        // Add audit entries after collection is complete
        context.Set<AuditLog>().AddRange(auditEntries);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private List<AuditLog> CollectAuditEntries(DbContext context, string userId)
    {
        var auditEntries = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is Order &&
                (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted))
            {
                // Create the audit entry
                var auditEntry = CreateAuditEntry(entry, userId);

                // Add the audit entry to the list (not modifying the tracked entries directly as it gave error)
                auditEntries.Add(auditEntry);
            }
        }

        return auditEntries;
    }

    private AuditLog CreateAuditEntry(EntityEntry entry, string userId)
    {
        var auditEntry = new AuditLog
        {
            TableName = entry.Entity.GetType().Name,
            Action = entry.State.ToString(),
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            KeyValues = GetPrimaryKey(entry),
            OldValues = entry.State == EntityState.Added ? "" : SerializeObject(entry.OriginalValues), // Empty for new entries
            NewValues = entry.State == EntityState.Deleted ? "" : SerializeObject(entry.CurrentValues)  // Empty for deleted entries
        };

        return auditEntry;
    }

    private string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
    }

    private string GetPrimaryKey(EntityEntry entry)
    {
        var key = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
        return key?.CurrentValue?.ToString() ?? string.Empty;
    }

    private string SerializeObject(PropertyValues values)
    {
        return JsonConvert.SerializeObject(values.Properties.ToDictionary(p => p.Name, p => values[p]));
    }
}
