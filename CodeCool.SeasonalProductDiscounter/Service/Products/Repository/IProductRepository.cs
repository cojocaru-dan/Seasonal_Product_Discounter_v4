using CodeCool.SeasonalProductDiscounter.Model.Products;
using CodeCool.SeasonalProductDiscounter.Model.Users;

namespace CodeCool.SeasonalProductDiscounter.Service.Products.Repository;

public interface IProductRepository
{
    int SoldProducts { get; set; }
    IEnumerable<Product> AvailableProducts { get; }
    bool Add(IEnumerable<Product> products);
    bool SetProductAsSold(Product product);
    bool SetProductuser_id(Product product, User user);
    void ClearProductsDatabase();
}
