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
        var query = "SELECT 1 FROM WareHouse WHERE WareHouse.IdWareHouse = @Id ";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int> DoesOrderExist(int idProduct, DateTime createdAt)
    {
        var query = "SELECT 1 FROM Order WHERE Order.IdProduct = @IdProduct AND Order.CreatedAt < @CreatedAt";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("IdProduct", idProduct);
        command.Parameters.AddWithValue("CreatedAt", createdAt);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int> AddProduct(WareHouseDTO wareHouseDto)
    {
        var query = @"INSERT INTO Product_Warehouse VALUES (@IdProduct, @IdWareHouse, @Amount, @CreatedAt); 
                        SELECT @@IDENTITY AS ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("IdProduct", wareHouseDto.IdProduct);
        command.Parameters.AddWithValue("IdWareHouse", wareHouseDto.IdWareHouse);
        command.Parameters.AddWithValue("Amount", wareHouseDto.Amount);
        command.Parameters.AddWithValue("CreatedAt", wareHouseDto.CreatedAt);

        await connection.OpenAsync();

        var id =  await command.ExecuteScalarAsync();
        
        if (id is null) throw new Exception();
        
        return Convert.ToInt32(id);
    }
}