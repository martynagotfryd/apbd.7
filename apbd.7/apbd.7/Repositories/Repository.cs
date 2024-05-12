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

    public async Task<int?> DoesOrderExist(int idProduct, int amount, DateTime createdAt)
    {
        var query = "SELECT IdOrder FROM [Order] o WHERE o.Amount = @Amount AND o.IdProduct = @IdProduct AND o.CreatedAt < @CreatedAt";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res != null ? (int?)res : null;
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
        var query = "UPDATE [Order] SET FulfilledAt = @Date WHERE IdOrder = @IdOrder";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();


        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Date", dateTime);
        command.Parameters.AddWithValue("@IdOrder", idOrder);

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


    public async Task<int> AddProduct(WareHouseDTO wareHouseDto, double price, int idOrder, DateTime dateTime)
    {
        var query = @"INSERT INTO Product_Warehouse VALUES (@IdWareHouse, @IdProduct, @IdOrder, @Amount, @Price ,@CreatedAt); 
                        SELECT @@IDENTITY AS ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", wareHouseDto.IdProduct);
        command.Parameters.AddWithValue("@IdWareHouse", wareHouseDto.IdWareHouse);
        command.Parameters.AddWithValue("@Amount", wareHouseDto.Amount);
        command.Parameters.AddWithValue("@CreatedAt", dateTime);
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        command.Parameters.AddWithValue("@Price", price);

        await connection.OpenAsync();

        var res =  await command.ExecuteScalarAsync();
        
        if (res is null) throw new Exception();
        
        return Convert.ToInt32(res);
    }

    public async Task<int> AddProductWithProc(WareHouseDTO wareHouseDto,  DateTime dateTime)
    {
        var query = @"EXEC AddProductToWarehouse @IdProduct = @IdProduct1, @IdWarehouse = @IdWareHouse1, @Amount = @Amount1, @CreatedAt = @CreatedAt1; SELECT @@IDENTITY AS ID ";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct1", wareHouseDto.IdProduct);
        command.Parameters.AddWithValue("@IdWareHouse1", wareHouseDto.IdWareHouse);
        command.Parameters.AddWithValue("@Amount1", wareHouseDto.Amount);
        command.Parameters.AddWithValue("@CreatedAt1", dateTime);
 
        await connection.OpenAsync();

        var res =  await command.ExecuteScalarAsync();
        
        if (res is null) throw new Exception();
        
        return Convert.ToInt32(res);
    }

    public async Task<int?> GetById(int id)
    {
        var query = "SELECT IdProductWarehouse FROM Product_Warehouse ph WHERE AND ph.IdProductWarehouse = @Id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res != null ? (int?)Convert.ToInt32(res) : null;    }

    public async Task AddProcedure()
    {
        var query = "CREATE PROCEDURE AddProductToWarehouse @IdProduct INT, @IdWarehouse INT, @Amount INT,  \n@CreatedAt DATETIME\nAS  \nBEGIN  \n   \n DECLARE @IdProductFromDb INT, @IdOrder INT, @Price DECIMAL(5,2);  \n  \n SELECT TOP 1 @IdOrder = o.IdOrder  FROM \"Order\" o   \n LEFT JOIN Product_Warehouse pw ON o.IdOrder=pw.IdOrder  \n WHERE o.IdProduct=@IdProduct AND o.Amount=@Amount AND  \n o.CreatedAt<@CreatedAt;  \n  \n SELECT @IdProductFromDb=Product.IdProduct, @Price=Product.Price FROM Product WHERE IdProduct=@IdProduct  \n   \n IF @IdProductFromDb IS NULL  \n BEGIN  \n  RAISERROR('Invalid parameter: Provided IdProduct does not exist', 18, 0);  \n  RETURN;  \n END;  \n  \n IF @IdOrder IS NULL  \n BEGIN  \n  RAISERROR('Invalid parameter: There is no order to fullfill', 18, 0);  \n  RETURN;  \n END;  \n   \n IF NOT EXISTS(SELECT 1 FROM Warehouse WHERE IdWarehouse=@IdWarehouse)  \n BEGIN  \n  RAISERROR('Invalid parameter: Provided IdWarehouse does not exist', 18, 0);  \n  RETURN;  \n END;  \n  \n SET XACT_ABORT ON;  \n BEGIN TRAN;  \n   \n UPDATE [Order] SET  \n FulfilledAt=@CreatedAt  \n WHERE IdOrder=@IdOrder;  \n  \n INSERT INTO Product_Warehouse(IdWarehouse,   \n IdProduct, IdOrder, Amount, Price, CreatedAt)  \n VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*@Price, @CreatedAt);  \n   \n SELECT @@IDENTITY AS NewId;\n   \n COMMIT;  \nEND; ";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
 
        await connection.OpenAsync();

        await command.ExecuteScalarAsync();
    }

    public async Task DropProcedure()
    {
        var query = "IF OBJECT_ID('AddProductToWarehouse', 'P') IS NOT NULL\nBEGIN\n    DROP PROCEDURE AddProductToWarehouse;\nEND";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
 
        await connection.OpenAsync();

        await command.ExecuteScalarAsync();
    }
}