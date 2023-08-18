using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            this.context = context;
            this.tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken already"); //A quick check to ensure that username isn't already taken upon registration. Need to prevent username enumeration here.
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(), //Make ToLower so things are standardized
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password.ToLower())), //We're passing it Bytes because it's expecting a Byte[], Make ToLower so things are standardized
                PasswordSalt = hmac.Key
            };

            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();

            return new UserDto {
                Username = user.UserName,
                Token = this.tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username); //Don't use find because we aren't using the Primary key
            if (user == null) return Unauthorized("Invalid Username");
            using var hmac = new HMACSHA512(user.PasswordSalt); //Remember the salt needs to be specified so the key is properly set
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password"); //If this is true, passwords aren't equivalent
            }

            //If we made it here, they did match
            return new UserDto {
                Username = user.UserName,
                Token = this.tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists (string username) 
        {
            return await this.context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); //x is an AppUser in this context, use ToLower() to ensure fair comparison for user input.
        }
    }
}