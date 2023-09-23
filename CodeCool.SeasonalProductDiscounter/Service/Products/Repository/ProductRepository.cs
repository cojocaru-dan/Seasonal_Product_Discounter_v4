using System.Text;
using CodeCool.SeasonalProductDiscounter.Model.Products;
using CodeCool.SeasonalProductDiscounter.Service.Logger;
using CodeCool.SeasonalProductDiscounter.Service.Persistence;
using CodeCool.SeasonalProductDiscounter.Utilities;
using CodeCool.SeasonalProductDiscounter.Model.Enums;
using CodeCool.SeasonalProductDiscounter.Model.Users;

namespace CodeCool.SeasonalProductDiscounter.Service.Products.Repository;

public class ProductRepository : SqLiteConnector, IProductRepository
{
    private readonly string _tableName;

    public IEnumerable<Product> AvailableProducts => GetAvailableProducts();
    public int SoldProducts { get; set; } = 0;

    public ProductRepository(string dbFile, ILogger logger) : base(dbFile, logger)
    {
        _tableName = DatabaseManager.ProductsTableName;
    }

    private IEnumerable<Product> GetAvailableProducts()
    {
        var query ="SELECT * FROM products";
        var ret = new List<Product>();

        try
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();
            Logger.LogInfo($"{GetType().Name} executing query: {query}");

            while (reader.Read())
            {
                var Id = (uint) reader.GetInt32(0);
                var Name = reader.GetString(2);
                var Color = (Color) Enum.Parse(typeof(Color), reader.GetString(3));
                var Season = (Season) Enum.Parse(typeof(Season), reader.GetString(4));
                var Price = reader.GetDouble(5);
                var Sold = reader.GetInt32(6) != 0;
                Product product = new Product(Id, Name, Color, Season, Price, Sold);
                ret.Add(product);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }
        Logger.LogInfo("The products have been extracted successfully!");
        return ret;
    }

    public bool Add(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            string query = $"INSERT INTO products VALUES( {product.Id}, 0, '{product.Name}', '{product.Color.ToString()}', '{product.Season.ToString()}', {product.Price}, {(product.Sold == true ? 1 : 0)} )";
            bool queryIsExecuted = ExecuteNonQuery(query);
            if (!queryIsExecuted) 
            {
                Logger.LogError($"There is a problem with adding product {product.Name} to database!");
                return false;
            }
        }
        Logger.LogInfo("The products have been added successfully!");
        return true;
    }

    public bool SetProductAsSold(Product product)
    {
        SoldProducts += 1;
        //Set the sold field in the database
        var query = $"UPDATE products SET sold = 1 WHERE id = {product.Id}";
        return ExecuteNonQuery(query);
    }
    public bool SetProductuser_id(Product product, User user)
    {
        var query = $"UPDATE products SET user_id = {user.Id} WHERE id = {product.Id}";
        return ExecuteNonQuery(query);
    }
    public void ClearProductsDatabase()
    {
        var query = $"DELETE FROM products";
        //
        using var connection = GetPhysicalDbConnection();
        using var command = GetCommand(query, connection);
        bool queryIsExecuted = ExecuteNonQuery(query);
        if (!queryIsExecuted)
        {
            Logger.LogError($"Can't clear products database!");
            return;
        }
        Logger.LogInfo($"The products database has been cleared successfully!");
        return;
    }
}
