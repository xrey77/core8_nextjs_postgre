using Npgsql;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Models;

namespace core8_nextjs_postgre.Services
{
    public interface IProductService {
        Task<IEnumerable<Product>> ListAll(int page);
        Task<IEnumerable<Product>> SearchAll(string key);
        Task<IEnumerable<Product>> Dataset();
        Task<int> TotPage();
    }

    public class ProductService : IProductService
    {

        private ApplicationDbContext _context;
        private readonly AppSettings _appSettings;

        public ProductService(ApplicationDbContext context,IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }        
        public async Task<int> TotPage() {
            var perPage = 5;
            var totRecs = await _context.Products.CountAsync();         
            int totPage = (int)Math.Ceiling((float)totRecs / perPage);
            return totPage;
        }

        public async Task<IEnumerable<Product>> ListAll(int page)
        {
            var perPage = 5;
            var offset = (page - 1) * perPage;

            // Use ToListAsync() for async execution
            var products = await _context.Products                                
                .OrderBy(b => b.Id)
                .Skip(offset)
                .Take(perPage)
                .ToListAsync();

            return products;            
        }

        public async Task<IEnumerable<Product>> SearchAll(string key)
        {
            var products = await _context.Products.FromSqlRaw("SELECT * FROM products WHERE lower(descriptions) LIKE '%" + key + "%'").ToListAsync();
            if (products.Count() == 0) {
               throw new AppException("Product not found");
            }
            return products;
        }

        public async Task<IEnumerable<Product>> Dataset()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }
    }
}