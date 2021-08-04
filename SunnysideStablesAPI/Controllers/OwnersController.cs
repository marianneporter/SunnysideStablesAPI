using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SunnysideStablesAPI.Data.Repository;
using SunnysideStablesAPI.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly IStablesRepo _repo;
        private readonly IMapper _mapper;

        public OwnersController(IStablesRepo repo,
                                IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwners()
        {
            var owners = await _repo.GetOwners();          

            return Ok(_mapper.Map<List<OwnerDto>>(owners));

        }
    }
}
