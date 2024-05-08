using apbd._7.Models.DTOs;
using Microsoft.Data.SqlClient;
namespace apbd._7.Repositories;

public class Repository : IRepository
{
    private readonly IConfiguration _configuration;

    public Repository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesProductExist(int id)
    {
        var query = "SELECT 1 FROM Product WHERE Product.IdProduct = @Id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesWareHouseExist(int id)
    {
        var query = "SELECT 1 FROM Warehouse WHERE WareHouse.IdWareHouse = @Id ";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int> DoesOrderExist(int idProduct, int amount, DateTime createdAt)
    {
        var query = "SELECT 1 FROM Order WHERE Order.IdProduct = @IdProduct AND Order.Amount = @Amount AND Order.CreatedAt < @CreatedAt";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        if (res is null) throw new Exception();

        return Convert.ToInt32(res);
    }

    public async Task<bool> IsOrderCompleted(int idOrder)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE Product_Warehouse.IdOrder = @IdOrder";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task UpgradeDate(int idOrder ,DateTime dateTime)
    {
        var query = "UPGRADE Order SET FullfilledAt = @Date";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        DateTime date = DateTime.Now;

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Date", dateTime);

        await connection.OpenAsync();

        await command.ExecuteScalarAsync();
    }

    public async Task<double> CalculatePrice(int idProduct, int amount)
    {
        var query = "SELECT Price FROM Product WHERE Product.IdProduct = @IdProduct";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        if (res is null) throw new Exception();
        
        return Convert.ToDouble(res) * amount;
        
    }


    public async Task<int> AddProduct(WareHouseDTO wareHouseDto, double price)
    {
        var query = @"INSERT INTO Product_Warehouse VALUES (@IdProduct, @IdWareHouse, @Amount, @CreatedAt); 
                        SELECT @@IDENTITY AS ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", wareHouseDto.IdProduct);
        command.Parameters.AddWithValue("@IdWareHouse", wareHouseDto.IdWareHouse);
        command.Parameters.AddWithValue("@Amount", wareHouseDto.Amount);
        command.Parameters.AddWithValue("@CreatedAt", wareHouseDto.CreatedAt);

        await connection.OpenAsync();

        var res =  await command.ExecuteScalarAsync();
        
        if (res is null) throw new Exception();
        
        return Convert.ToInt32(res);
    }
}