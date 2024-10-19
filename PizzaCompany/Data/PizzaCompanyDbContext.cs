using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PizzaCompany.DTOs;
using PizzaCompany.Models.Generated;
using System.Data;

namespace PizzaCompany.Data
{
    public class PizzaCompanyDbContext : DbContext
    {
        public PizzaCompanyDbContext(DbContextOptions<PizzaCompanyDbContext> options) 
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UsersRoles { get; set; } = null!;
        public DbSet<RoleChangeRequest> RoleChangeRequests { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; }

        //Get count of all orders
        public async Task<int> GetCount()
        {
            var count = new SqlParameter("@Count", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,
            };
            await Database.ExecuteSqlRawAsync("EXEC GetCount @Count output", count);
            return (int)count.Value;
        }
        //Get All orders with total count
        public async Task<(List<OrderStoredProcedureDTO>,int)> GetAllOrders()
        {
            var orderList = new List<OrderStoredProcedureDTO>();
            var count = new SqlParameter("@Count", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output,
            };
            using (var connection = Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "GetAllOrders";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(count);
                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        //Read the Order Table, ProductName and ProductPrice (1st query)
                        while(await reader.ReadAsync())
                        {
                            orderList.Add(new OrderStoredProcedureDTO
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                ProductPrice = reader.IsDBNull(reader.GetOrdinal("ProductPrice"))?
                                                (decimal?)null
                                                : reader.GetDecimal(reader.GetOrdinal("ProductPrice")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            });
                        }
                    }
                }
            }
            return (orderList, (int)count.Value);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Creating one to many relationship for Order and User
            modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            //Creating one to many relationship for Order and Product
            modelBuilder.Entity<Order>()
            .HasOne(o => o.Product)
            .WithMany()
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            //create composite key
            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

            //Creating many to many relationship between User and Role using
            //UserRole join table
            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

            //One to many relationship between RoleChangeRequest and User
            modelBuilder.Entity<RoleChangeRequest>()
            .HasOne(r => r.User)
            .WithMany() 
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);      

            //One to many relationship between RoleChangeRequest and Role
            modelBuilder.Entity<RoleChangeRequest>()
            .HasOne(r => r.Role)
            .WithMany()
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
            
            //Configuring Property Password with maxLength - 100
            modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(256);
        }
    }
}
