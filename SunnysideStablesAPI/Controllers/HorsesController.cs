using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SunnysideStablesAPI.Data.Repository;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HorsesController : ControllerBase
    {
        private readonly IStablesRepo _repo;
        private readonly IMapper _mapper;

        public HorsesController(IStablesRepo repo,
                                IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetHorses(int pageIndex=0, int pageSize=3)
        {
            var horses = await _repo.GetHorses(true, pageIndex, pageSize);

          
            return Ok(_mapper.Map<List<HorseDto>>(horses));
          
        }
        
        [HttpGet("count")]
        public async Task<IActionResult> GetHorseCount()
        {
            var horseCount = await _repo.GetHorseCount();

            return Ok(horseCount);        
         
        }


        [HttpPost]
        public async Task<IActionResult> AddHorse([FromForm] HorseAddUpdateDto horseAddUpdateDto)
        {
            Horse horseToAdd = _mapper.Map<Horse>(horseAddUpdateDto);

            _repo.Add(horseToAdd);

            var addSuccess = await  _repo.Commit();

            if (!addSuccess)
            {
                return StatusCode(500);
            }

            //add cottage owner entitie(s)  

            List<HorseOwner> horseOwners = horseAddUpdateDto.OwnerIds.Select(o =>
                                new HorseOwner
                                {
                                    HorseId = horseToAdd.Id,
                                    OwnerId = o
                                }).ToList();


            _repo.AddHorseOwners(horseOwners);

             await _repo.Commit();

 

            // photo into blob storage if required
            // 

            return StatusCode(201);

        }
    }
}
