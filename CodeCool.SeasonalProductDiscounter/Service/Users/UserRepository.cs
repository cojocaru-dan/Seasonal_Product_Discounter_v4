using System.Data;
using System.Runtime.InteropServices;
using CodeCool.SeasonalProductDiscounter.Model.Users;
using CodeCool.SeasonalProductDiscounter.Service.Logger;
using CodeCool.SeasonalProductDiscounter.Service.Persistence;
using CodeCool.SeasonalProductDiscounter.Utilities;

namespace CodeCool.SeasonalProductDiscounter.Service.Users;

public class UserRepository : SqLiteConnector, IUserRepository
{
    public UserRepository(string dbFile, ILogger logger) : base(dbFile, logger)
    {
    }

    public IEnumerable<User> GetAll()
    {
        var query = @$"SELECT * FROM {DatabaseManager.UsersTableName}";
        var ret = new List<User>();

        try
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                ret.Add(user);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }

        return ret;
    }

    public bool Add(User user)
    {
        string query = $"INSERT INTO users(user_name, password) VALUES( '{user.UserName}', '{user.Password}' )";
        bool queryIsExecuted = ExecuteNonQuery(query);
        if (!queryIsExecuted)
        {
            Logger.LogError($"There is a problem with adding user {user.UserName} to database!");
            return false;
        }
        Logger.LogInfo($"The user {user.UserName} has been added successfully!");
        return true;
    }

    public User Get(string name)
    {
        var query = $"SELECT * FROM users WHERE user_name = '{name}'";
        User user = new User(int.MinValue, "not exist", "not exist");
        try
        {
            //
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();
            Logger.LogInfo($"{GetType().Name} executing query: {query}");
            while (reader.Read())
            {
                var Id = TypeConverters.ToInt(reader.GetInt32(0)); 
                var UserName = TypeConverters.ToString(reader.GetString(1)); 
                var Password = TypeConverters.ToString(reader.GetString(2));
                user = new User(Id, UserName, Password);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }
        return user;
    }
    public void ClearUsersDatabase()
    {
        var query = $"DELETE FROM users";
        //
        using var connection = GetPhysicalDbConnection();
        using var command = GetCommand(query, connection);
        bool queryIsExecuted = ExecuteNonQuery(query);
        if (!queryIsExecuted)
        {
            Logger.LogError($"Can't clear users database!");
            return;
        }
        Logger.LogInfo($"The users database has been cleared successfully!");
        return;
    }
}
