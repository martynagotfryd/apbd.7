using apbd._7.Models.DTOs;
using apbd._7.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpPost("Add")]
        public async Task<IActionResult> AddProduct(WareHouseDTO wareHouseDto)
        {
            DateTime dateTime = DateTime.Now;
                
            if (!await _repository.DoesProductExist(wareHouseDto.IdProduct))
            {
                return NotFound("Product not found");
            }

            if (!await _repository.DoesWareHouseExist(wareHouseDto.IdWareHouse))
            {
                return NotFound("WareHouse not found");
            }
            
            var idOrder = await _repository.DoesOrderExist(wareHouseDto.IdProduct, wareHouseDto.Amount,dateTime);
            if (idOrder == null)
            {
                return NotFound("Order not found");
            }
            Console.WriteLine("PATRZzzzzzzzzzzzzzzzzzzzz");

            Console.WriteLine(idOrder);
            if (await _repository.IsOrderCompleted(Convert.ToInt32(idOrder)))
            {
                return BadRequest("Order is completed");
            }


            try
            {
                await _repository.UpgradeDate(Convert.ToInt32(idOrder), dateTime);

                
            }
            catch (Exception e)
            {
                Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                throw;
            }

            double price = await _repository.CalculatePrice(wareHouseDto.IdProduct, wareHouseDto.Amount);

            int id = await _repository.AddProduct(wareHouseDto, price, Convert.ToInt32(idOrder), dateTime);

            return CreatedAtAction(nameof(GetProductWarehouseById), new { id }, new { Id = id }); 
        }

        [HttpPost("AddWithP")]
        public async Task<IActionResult> AddProductWithProc(WareHouseDTO wareHouseDto)
        {
            await _repository.DropProcedure();
            await _repository.AddProcedure();
            
            DateTime dateTime = DateTime.Now;
            
            int id = await _repository.AddProductWithProc(wareHouseDto, dateTime);

            return CreatedAtAction(nameof(GetProductWarehouseById), new { id }, new { Id = id }); 

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductWarehouseById(int id)
        {
            var result = await _repository.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
       
    }
}
