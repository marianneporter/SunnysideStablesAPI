using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SunnysideStablesAPI.Data.Repository;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using SunnysideStablesAPI.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnersController : ControllerBase
    {
        private readonly IStablesRepo _repo;
        private readonly IMapper _mapper;
        private UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public OwnersController(IStablesRepo repo,
                                IMapper mapper,
                                UserManager<User> userManager,
                                IConfiguration config)
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwners()
        {
            var owners = await _repo.GetOwners();          

            return Ok(_mapper.Map<List<OwnerDto>>(owners));

        }


        [HttpPost]
        public async Task<IActionResult> AddOwner(OwnerDto ownerDto)
        {
            var owner = new Owner()
            {
                Email = ownerDto.Email,
                UserName = ownerDto.Email,
                FirstName = ownerDto.FirstName,
                LastName = ownerDto.LastName
            };

            var result = _userManager.CreateAsync(owner, _config["InitialOwnerPassword"]).Result;

            if (result != IdentityResult.Success)
            {
                return StatusCode(500);
            }

            var addedOwner = _userManager.FindByEmailAsync(ownerDto.Email).Result;

            _userManager.AddToRolesAsync(addedOwner, new string[] { "Client" }).Wait();

            return Ok(_mapper.Map<OwnerDto>(addedOwner));
        }
    }
}
