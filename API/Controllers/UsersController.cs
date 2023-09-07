using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IPhotoService photoService;
    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.photoService = photoService;
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
        var username = User.GetUsername(); //Get the username from the token
        var user = await this.userRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        this.mapper.Map(memberUpdateDto, user);

        if (await this.userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file) 
    {
        var user = await this.userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return NotFound(); //Defensive code to make sure that we have a user before continuing

        var result = await this.photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message); //Defensive code to ensure that we didn't receive an error before continuing

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true; //If this is their first photo upload, make it their main photo by default

        user.Photos.Add(photo);

        if (await this.userRepository.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, this.mapper.Map<PhotoDto>(photo));
        } 

        return BadRequest("Problem adding photo"); //If we make it to this point then there was a problem saving the photo.
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int PhotoId)
    {
        var user = await this.userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return NotFound();
        var photo = user.Photos.FirstOrDefault(x => x.Id == PhotoId);
        if (photo == null) return NotFound();
        if (photo.IsMain) return BadRequest("This photo is already your main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null)
        {
            currentMain.IsMain = false;
            photo.IsMain = true;
        }

        if (await this.userRepository.SaveAllAsync())
        {
            return NoContent();
        } 

        return BadRequest("Problem setting the main photo. Unable to perform update.");
        

    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await this.userRepository.GetUserByUsernameAsync(User.GetUsername());
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound("The photo referenced does not currently exist.");
        if (photo.IsMain) return BadRequest("You cannot delete your main photo!");

        //This is a check to make sure the photo isn't a seeded photo and is actually stored in Cloudinary
        if (photo.PublicId != null)
        {
            var result = await this.photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await this.userRepository.SaveAllAsync()) return Ok(); //Return Ok if all the changes were saved successfully and we made it this far.

        return BadRequest("Problem occured while deleting the photo!");
    }
}
