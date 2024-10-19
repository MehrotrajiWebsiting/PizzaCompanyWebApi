using Microsoft.AspNetCore.Mvc;
using PizzaCompany.Models.Generated;

namespace PizzaCompany.Interface
{
    public interface IProductRepository
    {
        Task<Product> GetByName(String name);
        Task<IEnumerable<Product>> GetAll();
        Task<Product> Create(Product product);
        void Update(Product existingProduct,Product product);
        void Delete(Product product);
        Task<IEnumerable<object>> GetUserWithProduct(int Id);
        Task<IEnumerable<object>> GetAllUserProduct();
    }
}
