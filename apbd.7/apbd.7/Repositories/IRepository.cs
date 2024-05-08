using apbd._7.Models.DTOs;

namespace apbd._7.Repositories;

public interface IRepository
{
    Task<bool> DoesProductExist(int id);
    Task<bool> DoesWareHouseExist(int id);
    Task<int?> DoesOrderExist(int idProduct, int amount, DateTime createdAt);
    Task<bool> IsOrderCompleted(int idOrder);
    Task UpgradeDate(int idOrder, DateTime dateTime);
    Task<double> CalculatePrice(int idProduct, int amount);
    Task<int> AddProduct(WareHouseDTO wareHouseDto, double price, int idOrder, DateTime dateTime);
}