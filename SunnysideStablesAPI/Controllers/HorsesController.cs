﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SunnysideStablesAPI.Data.Repository;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using SunnysideStablesAPI.UtilityServices.PhotoBlobs;
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
        private readonly IPhotoBlobService _photoService;

        public HorsesController(IStablesRepo repo,
                                IMapper mapper,
                                IPhotoBlobService photoService)
        {
            _repo = repo;
            _mapper = mapper;
            _photoService = photoService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetHorses(int pageIndex=0, int pageSize=3, string search="")
        {
            var horseCount = await _repo.GetHorseCount();

            var horses = await _repo.GetHorses(true, pageIndex, pageSize, search);

            return Ok ( new { count = horseCount, horses =_mapper.Map<List<HorseDto>>(horses) });
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHorseById(int id)
        {
            var horse = await _repo.GetHorseById(id);

            return Ok(_mapper.Map<HorseDto>(horse));

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

            //add horse owner entitie(s)  

            List<HorseOwner> horseOwners = horseAddUpdateDto.OwnerIds.Select(o =>
                                new HorseOwner
                                {
                                    HorseId = horseToAdd.Id,
                                    OwnerId = o
                                }).ToList();

            _repo.AddHorseOwners(horseOwners);            

            // image to blob storage

            if (horseAddUpdateDto.ImageFile != null)
            {
                var photoUrl = await SavePhoto(null, horseAddUpdateDto.ImageFile, horseToAdd.ModifiedDate, horseToAdd.Name);

                if (!String.IsNullOrEmpty(photoUrl)) // photo saved successfully
                {
                    horseToAdd.ImageUrl = photoUrl;
                }
            }

            await _repo.Commit();

            return Created("~api/horses", new { id = horseToAdd.Id, name = horseToAdd.Name });

        }

        [HttpPatch]
        public async Task<IActionResult> UpdateHorse([FromForm] HorseAddUpdateDto horseAddUpdateDto)
        {
            Horse horseToUpdate = await _repo.GetHorseById(horseAddUpdateDto.Id);
            if (horseToUpdate == null)
            {
                return NotFound($"Horse with Name {horseAddUpdateDto.Name} could not be found");
            }

            horseToUpdate = _mapper.Map<HorseAddUpdateDto, Horse>(horseAddUpdateDto, horseToUpdate);
            horseToUpdate.ModifiedDate = DateTime.Now;

            if (horseAddUpdateDto.ImageFile != null)
            {
                var photoUrl = await SavePhoto(horseToUpdate.ImageUrl, horseAddUpdateDto.ImageFile, horseToUpdate.ModifiedDate, horseToUpdate.Name);

                if (!String.IsNullOrEmpty(photoUrl)) // old  photo deleted and new one saved successfully
                {
                    horseToUpdate.ImageUrl = photoUrl;
                }
            }

            horseToUpdate.ModifiedDate = DateTime.Now;
 
            var updateSuccess =await _repo.Commit();

            if (!updateSuccess)
            {
                return StatusCode(500);
            }

            updateSuccess = await CheckAndUpdateOwners(horseToUpdate, horseAddUpdateDto.OwnerIds.ToArray());

            return updateSuccess ? NoContent() : StatusCode(500);

        }

        private async Task<string> SavePhoto(string oldImageUrl, IFormFile uploadedPhoto, DateTime modifiedDate, string horseName)  {

            if (oldImageUrl != null)
            {
                await this._photoService.RemovePhotoBlob(oldImageUrl);
            }

            var blobUrl = await this._photoService.AddPhotoBlob(uploadedPhoto, horseName, modifiedDate);
 
            return blobUrl;
        }

        private async Task<bool> CheckAndUpdateOwners(Horse horseToUpdate, int[] ownerIds)
        {
            var ownersChanged = false;
            var currentOwnerIds = horseToUpdate.HorseOwner.Select(i => i.OwnerId).ToList();
            for (int i = 0; i<ownerIds.Length; i++)
            {
                if (!currentOwnerIds.Contains(ownerIds[i]))
                {
                   
                    HorseOwner horseOwner = new HorseOwner()
                    {
                        HorseId = horseToUpdate.Id,
                        OwnerId = ownerIds[i]
                    };
                    _repo.AddHorseOwner(horseOwner);
                    ownersChanged = true;
                }
            }

            var deletedOwnerIds = currentOwnerIds.Where(o => !ownerIds.Contains(o)).ToList();
            for (int i = 0; i< deletedOwnerIds.Count; i++)
            {
                var ownerToRemove = horseToUpdate.HorseOwner.First(o => o.OwnerId == deletedOwnerIds[i]);
                ownersChanged = true;
                _repo.DeleteHorseOwner(ownerToRemove);
            }

           
            if (ownersChanged)
            {
                var updateSuccess = await _repo.Commit();
                return updateSuccess;
            }

            return true;
        } 
    }
}
