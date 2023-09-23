using System.Data;
using CodeCool.SeasonalProductDiscounter.Model.Products;
using CodeCool.SeasonalProductDiscounter.Model.Transactions;
using CodeCool.SeasonalProductDiscounter.Model.Users;
using CodeCool.SeasonalProductDiscounter.Service.Logger;
using CodeCool.SeasonalProductDiscounter.Service.Persistence;
using CodeCool.SeasonalProductDiscounter.Utilities;

namespace CodeCool.SeasonalProductDiscounter.Service.Transactions.Repository;

public class TransactionRepository : SqLiteConnector, ITransactionRepository
{
    private readonly string _tableName;

    public TransactionRepository(string dbFile, ILogger logger) : base(dbFile, logger)
    {
        _tableName = DatabaseManager.TransactionsTableName;
    }

    public bool Add(Transaction transaction)
    {
        //
        string query = $"INSERT INTO transactions VALUES( {transaction.Id}, '{transaction.Date.ToString()}', {transaction.User.Id}, {transaction.Product.Id}, {transaction.PricePaid} )";
        bool queryIsExecuted = ExecuteNonQuery(query);
        if (!queryIsExecuted)
        {
            Logger.LogError($"There is a problem with adding transaction number {transaction.Id} to database!");
            return false;
        }
        Logger.LogInfo($"The transaction has been added successfully!");
        return true;
    }

    public IEnumerable<Transaction> GetAll()
    {
        var query = "SELECT * FROM products INNER JOIN users ON products.user_id = users.id INNER JOIN transactions ON users.id = transactions.user_id";

        try
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();
            Logger.LogInfo($"{GetType().Name} executing query: {query}");

            var dt = new DataTable();

            //This is required otherwise the DataTable tries to force the DB constrains on the result set, which can cause problems in some cases (e.g. UNIQUE)
            using var ds = new DataSet { EnforceConstraints = false };
            ds.Tables.Add(dt);
            dt.Load(reader);
            ds.Tables.Remove(dt);

            var lst = new List<Transaction>();
            foreach (DataRow row in dt.Rows)
            {
                var user = ToUser(row);
                var product = ToProduct(row);
                var transaction = ToTransaction(row, user, product);

                lst.Add(transaction);
            }
            return lst;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static User ToUser(DataRow row)
    {
        var Id = TypeConverters.ToInt(row["id1"]);
        var UserName = TypeConverters.ToString(row["user_name"]);
        var Password = TypeConverters.ToString(row["password"]);

        return new User(Id, UserName, Password);
    }

    private static Product ToProduct(DataRow row)
    {
        var Id = (uint) TypeConverters.ToInt(row["id"]);
        var Name = TypeConverters.ToString(row["name"]);
        var Color = TypeConverters.GetColorEnum(row["color"].ToString());
        var Season = TypeConverters.GetSeasonEnum(row["season"].ToString());
        var Price = TypeConverters.ToDouble(row["price"]);
        var Sold = TypeConverters.ToInt(row["sold"]) == 1;

        return new Product(Id, Name, Color, Season, Price, Sold);

    }

    private static Transaction ToTransaction(DataRow row, User user, Product product)
    {
        var Id = TypeConverters.ToInt(row["id2"]);
        var Date = TypeConverters.ToDateTime(row["date"].ToString());
        var PricePaid = TypeConverters.ToDouble(row["price_paid"]);

        return new Transaction(Id, Date, user, product, PricePaid);
    }
    public void ClearTransactionsDatabase()
    {
        var query = $"DELETE FROM transactions";
        //
        using var connection = GetPhysicalDbConnection();
        using var command = GetCommand(query, connection);
        bool queryIsExecuted = ExecuteNonQuery(query);
        if (!queryIsExecuted)
        {
            Logger.LogError($"Can't clear transactions database!");
            return;
        }
        Logger.LogInfo($"The transactions database has been cleared successfully!");
        return;
    }
}
