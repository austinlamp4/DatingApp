using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
//Route is going to be https://localhost:5000/api/users
[Route("api/[controller]")] //[controller] makes it so that it's api/<firstpart of the name of the file without controller>, so /api/users
public class UsersController : ControllerBase
{
    private readonly DataContext context;
    public UsersController(DataContext context)
    {
        this.context = context;
    }

    [HttpGet] //Creating a GET request to get all users, defaults to /api/users since that's what's configured at the [Route] and we didn't specify anything additional like below.
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync(); //Needs error handling
        return users;
    }

    [HttpGet("{id}")] // /api/users/2
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await this.context.Users.FindAsync(id);
        if (user != null) {
            return user;
        } else { //Need to deal with a user that doesn't exist..
            throw new Exception("test"); //Need to learn to throw just an exception page...
        }
    }
}
