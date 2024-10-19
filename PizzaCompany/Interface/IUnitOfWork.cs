namespace PizzaCompany.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IOrderRepository OrderRepository { get; }
        IProductRepository ProductRepository { get; }
        IRoleRepository RoleRepository { get; }
        IRoleChangeRequestRepository RoleChangeRequestRepository { get; }

        Task<bool> SaveChangesAsync();
    }
}
