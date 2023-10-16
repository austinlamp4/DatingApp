using API.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    //Route is going to be https://localhost:5000/api/users
    [Route("api/[controller]")] //[controller] makes it so that it's api/<firstpart of the name of the file without controller>, so /api/users
    public class BaseApiController : ControllerBase
    {
        
    }
}