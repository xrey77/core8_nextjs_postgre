using Npgsql;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Models;

namespace core8_nextjs_postgre.Services
{
    public interface IProductService {
        IEnumerable<Product> ListAll(int page);
        IEnumerable<Product> SearchAll(string key);
        IEnumerable<Product> Dataset();

        int TotPage();
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
        public int TotPage() {
            var perpage = 5;
            var totrecs = _context.Products.Count();
            int totpage = (int)Math.Ceiling((float)(totrecs) / perpage);
            return totpage;
        }
        public IEnumerable<Product> ListAll(int page)
        {
            var perpage = 5;
            var offset = (page -1) * perpage;

            var products = _context.Products                                
                .OrderBy(b => b.Id)
                .Skip(offset)
                .Take(perpage)
                .ToList();

            return products;
        }

        public IEnumerable<Product> SearchAll(string key)
        {
            var columnName = $"descriptions";
            var columnValue = new NpgsqlParameter("columnValue", "%" + key + "%");
            var sql = $"SELECT * FROM [Products] WHERE {columnName} LIKE @columnValue";
            var products = _context.Products.FromSqlRaw(sql, columnValue).ToList();
            return products;
        }

        public IEnumerable<Product> Dataset()
        {
            var products = _context.Products.ToList();
            return products;
        }
    }
}