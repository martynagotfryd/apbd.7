using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apbd._7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAnimalRepository _animalRepository;
    
        public WareHouseController(IConfiguration configuration, IAnimalRepository animalRepository)
        {
            _configuration = configuration;
            _animalRepository = animalRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> get()
        {
            
        }
    }
}
