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
        
        
    }
}
