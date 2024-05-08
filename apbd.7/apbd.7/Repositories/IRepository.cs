using apbd._7.Models.DTOs;

namespace apbd._7.Repositories;

public interface IRepository
{
    Task<bool> DoesProductExist(int id);
    Task<bool> DoesWareHouseExist(int id);
    Task<int> DoesOrderExist(int idProduct, DateTime createdAt);
    Task<bool> IsOrderCompleted(int idOrder);

    Task<int> AddProduct(WareHouseDTO wareHouseDto);
}