using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    [HttpGet] //Creating a GET request to get all users, defaults to /api/users since that's what's configured at the [Route] and we didn't specify anything additional like below.
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await this.userRepository.GetUsersAsync();
        var usersToReturn = this.mapper.Map<IEnumerable<MemberDto>>(users);
        return Ok(usersToReturn);
    }

    [HttpGet("{username}")] // /api/users/2 -- this adds on to the base route, which is now defined in BaseApiController
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await this.userRepository.GetUserByUsernameAsync(username);
        return this.mapper.Map<MemberDto>(user);
    }
}
