using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using SunnysideStablesAPI.Models.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
           
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);           

            if (result.Succeeded)
            {

                var generatedToken = await GenerateJwtToken(user);

                return Ok(new
                {
                    token = generatedToken,
                    user = new {
                        firstName = user.FirstName,
                        lastName = user.LastName
                    } 
                });
            }

            return Unauthorized();          

        }

        //[Authorize(Roles = "Manager")]
        //[HttpPost("register")]
        //public async Task<IActionResult> Register(RegisterDto registerDto)
        //{

        //    var user = await _userManager.FindByEmailAsync(registerDto.Email);

        //    if (user != null)
        //    {
        //        return Conflict("There is already an account with this email");
        //    }

        //    IdentityResult result;        

        //    if (registerDto.Role == "Admin")
        //    {
        //        var adminUser = new Staff()
        //        {
        //            Email = registerDto.Email,
        //            UserName = registerDto.Email,
        //            FirstName = registerDto.FirstName,
        //            LastName = registerDto.LastName,
        //            JobTitle = registerDto.JobTitle
        //        };               

        //        result = _userManager.CreateAsync(adminUser, registerDto.Password).Result;

        //        if (result != IdentityResult.Success)
        //        {
        //            return BadRequest("User could not be created");
        //        }

        //        var addedUser = _userManager.FindByEmailAsync(adminUser.Email).Result;

        //        _userManager.AddToRolesAsync(addedUser, new string[] { "Admin" }).Wait();
        //    } else
        //    {
        //        // To do...  create new owner user
        //        var ownerUser = new Owner()
        //        {
        //            Email = registerDto.Email,
        //            UserName = registerDto.Email,
        //            FirstName = registerDto.FirstName,
        //            LastName = registerDto.LastName
        //        };

        //        result = _userManager.CreateAsync(ownerUser, registerDto.Password).Result;

        //        if (result != IdentityResult.Success)
        //        {
        //            return BadRequest("User could not be created");
        //        }

        //        var addedUser = _userManager.FindByEmailAsync(ownerUser.Email).Result;

        //        _userManager.AddToRolesAsync(ownerUser, new string[] { "Client" }).Wait();
        //    }

        //    return Ok("User Registered");
        //}

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create new security key from config value, and then a new SigningCredentials object using this
            // along with a hashing algorithm.
            var jwtKeyFromConfig = _config.GetSection("JwtKey").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKeyFromConfig));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Set up token descriptor and  create token with token handler
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
