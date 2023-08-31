using System.Security.Claims;
using API.DTOs;
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
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        return Ok(await this.userRepository.GetMembersAsync());
    }

    [HttpGet("{username}")] // /api/users/2 -- this adds on to the base route, which is now defined in BaseApiController
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await this.userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //Get the username from the token
        var user = await this.userRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        this.mapper.Map(memberUpdateDto, user);

        if (await this.userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }
}
