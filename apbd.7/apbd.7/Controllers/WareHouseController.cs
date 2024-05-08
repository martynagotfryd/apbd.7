using apbd._7.Models.DTOs;
using apbd._7.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace apbd._7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        private readonly IRepository _repository;
    
        public WareHouseController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(WareHouseDTO wareHouseDto)
        {
            if (!await _repository.DoesProductExist(wareHouseDto.IdProduct))
            {
                return NotFound("Product not found");
            }

            if (!await _repository.DoesWareHouseExist(wareHouseDto.IdWareHouse))
            {
                return NotFound("WareHouse not found");
            }

            try 
            {
                var idOrder = await _repository.DoesOrderExist(wareHouseDto.IdProduct, wareHouseDto.Amount,wareHouseDto.CreatedAt);

                if (await _repository.IsOrderCompleted(idOrder))
                {
                    return BadRequest("Order is completed");
                }
                try
                {
                    await _repository.UpgradeDate(idOrder, wareHouseDto.CreatedAt);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return NotFound("Order not found");
            }

            double price = await _repository.CalculatePrice(wareHouseDto.IdProduct, wareHouseDto.Amount);

            int id = await _repository.AddProduct(wareHouseDto, price);

            return Created();
        } 
    }
}
