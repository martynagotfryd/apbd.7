using apbd._7.Models.DTOs;
using apbd._7.Repositories;
using Microsoft.AspNetCore.Http;
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

            if (!await _repository.DoesOrderExist(wareHouseDto.IdProduct, wareHouseDto.CreatedAt))
            {
                return NotFound("Order not found");
            }

            int id = await _repository.AddProduct(wareHouseDto);

            return Created();
        }
    }
}
