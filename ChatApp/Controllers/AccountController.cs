using System.Security.Cryptography;
using System.Text;
using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUsers> _userManager;
    private readonly SignInManager<AppUsers> _signInManager;
    // private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    // public AccountController( DbContext context, ITokenService tokenService)
    public AccountController( UserManager<AppUsers> userManager, SignInManager<AppUsers> signInManager, ITokenService tokenService)

    {
        _userManager = userManager;
        _signInManager = signInManager;
        // _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken.");
        // using var hmac = new HMACSHA512();
        var user = new AppUsers
        {
            UserName = registerDto.UserName.ToLower(),
            Email = registerDto.Email.ToLower(),
            HostOfRooms = new List<AppRooms>(),
            AppMessages = new List<AppMessages>(),
            ParticipantOfRooms = new List<AppRooms>(),
            // PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            // PasswordSalt = hmac.Key
        };
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResults = await _userManager.AddToRoleAsync(user, "Member");
        if (!roleResults.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        loginDto.UserName = loginDto.UserName.ToLower();
        var user = await _userManager.Users.SingleOrDefaultAsync(x=> x.UserName == loginDto.UserName);
        if (user == null) return Unauthorized("Invalid username.");

        // using var hmac = new HMACSHA512(user.PasswordSalt);
        // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        //
        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        // }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) return Unauthorized("Password or Username wrong!");

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            AvatarUrl = user.AvatarUrl
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}