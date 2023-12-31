using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
   
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.userName)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();
            var user = new AppUser{
                Name = registerDto.userName,
                PasswordHash =hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt =hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                userName = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }    
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login (LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Name == loginDto.userName);

            if(user == null) return Unauthorized("Invalid username");

            using var  hmac = new HMACSHA512(user.PasswordSalt);
            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i=0; i<ComputeHash.Length; i++)
            {
                if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password"); 
            }
             return new UserDto
            {
                userName = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }   
        private async Task<bool> UserExists(string Name)
        {
            return await _context.Users.AnyAsync(x => x.Name == Name.ToLower());
        }
   
    }
}