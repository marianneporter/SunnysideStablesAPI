using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SunnysideStablesAPI.Data.Repository;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using SunnysideStablesAPI.Models.Identity;
using System.Collections.Generic;
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetOwners()
        {
            var owners = await _repo.GetOwners();          

            return Ok(_mapper.Map<List<OwnerDto>>(owners));

        }

        [AllowAnonymous]
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

            var result = await _userManager.CreateAsync(owner, _config["InitialOwnerPassword"]);

            if (result != IdentityResult.Success)
            {
                return StatusCode(500);
            }

            var addedOwner = await _userManager.FindByEmailAsync(ownerDto.Email);

            await _userManager.AddToRolesAsync(addedOwner, new string[] { "Client" });

            return Ok(_mapper.Map<OwnerDto>(addedOwner));
        }
    }
}
