using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
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


/*
    [HttpGet] //Creating a GET request to get all users, defaults to /api/users since that's what's configured at the [Route] and we didn't specify anything additional like below.
    public async Task<ActionResult<IEnumerable<String>>> GetUsers()
    {
        var users = await context.Users.ToListAsync(); //Needs error handling
        List<String> list = new List<String>();
        foreach (AppUser user in users) {
            list.Add(user.UserName);
        }
        ActionResult<IEnumerable<String>> returnable = list;
        return returnable;
    }
*/

    [HttpGet("{id}")] // /api/users/2 -- this adds on to the base route, which is now defined in BaseApiController
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
