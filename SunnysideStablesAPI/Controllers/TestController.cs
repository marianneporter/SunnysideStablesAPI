using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SunnysideStablesAPI.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SunnysideStablesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TestController(IConfiguration config)
        {
            _config = config;
        }
        
        [HttpGet("getFruits")]
        [AllowAnonymous]      
        public ActionResult GetFruits()
        {
            List<string> myList = new List<string>()
            {
                "apples", "pears", "strawberries", "blackcurrents"
            };
            return Ok(myList);
        }

        [HttpGet("getFruitsAuth")]
        public ActionResult GetFruitsAuth()
        {
            List<string> myList = new List<string>()
            {
                "bananas", "oranges", "nectarines", "pineapples"
            };
            return Ok(myList);
        }

        [AllowAnonymous]
        [HttpPost("getToken")]
        public ActionResult GetToken([FromBody] LoginDto loginDto )
        {
            if (loginDto.Email=="tibbles@gmail.com" && loginDto.Password=="tibblesIsGreat")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JwtKey"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, loginDto.Email)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
            } 
            else
            {
                return Unauthorized("computer says no");
            }
        }
    }
}
