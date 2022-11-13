using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SunnysideStablesAPI.Data.Repository;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;

using SunnysideStablesAPI.UtilityServices.PhotoBlobs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SunnysideStablesAPI.Shared;
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

       
        [HttpGet]
        public async Task<IActionResult> GetHorses(int pageIndex=0, int pageSize=3, string search="")
        {
  
            HorseListData horseData = await _repo.GetHorses(true, pageIndex, pageSize, search);

            return Ok ( new { countAll = horseData.CountAll,
                              searchCount = horseData.SearchCount,
                              horses =_mapper.Map<List<HorseDto>>(horseData.Horses) });
            
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

            horseToAdd.ModifiedBy = Utility.GetCurrentUser(User.Claims.FirstOrDefault().Value);

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

            var photoUploaded = false;
            if (horseAddUpdateDto.ImageFile != null)
            {
                var photoUrl = await SavePhoto(null, horseAddUpdateDto.ImageFile, horseToAdd.ModifiedDate, horseToAdd.Name);
                if (photoUrl != null)
                {
                    photoUploaded = true;
                    horseToAdd.ImageUrl = photoUrl;
                }              
            }
 
            await _repo.Commit();

            return Created("~api/horses", new { id = horseToAdd.Id, name = horseToAdd.Name, photoUploaded = photoUploaded });

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

            horseToUpdate.ModifiedBy = Utility.GetCurrentUser(User.Claims.FirstOrDefault().Value);

            var photoUploaded = false;
            if (horseAddUpdateDto.ImageFile != null)
            {
                var photoUrl = await SavePhoto(horseToUpdate.ImageUrl, horseAddUpdateDto.ImageFile, horseToUpdate.ModifiedDate, horseToUpdate.Name);
                if (photoUrl != null)
                {
                    photoUploaded = true;
                    horseToUpdate.ImageUrl = photoUrl;
                }
            } 

            var updateSuccess =await _repo.Commit();

            if (!updateSuccess)
            {
                return StatusCode(500);
            }

            updateSuccess = await CheckAndUpdateOwners(horseToUpdate, horseAddUpdateDto.OwnerIds.ToArray());
  
            return updateSuccess ? Ok( new { photoUploaded } ) : StatusCode(500);

        }
         
        private async Task<string> SavePhoto(string oldImageUrl, IFormFile uploadedPhoto, DateTime modifiedDate, string horseName)
        {
            string blobUrl = null;
            bool formatError = false;
            try
            {
                using (var image = Image.FromStream(uploadedPhoto.OpenReadStream()))
                {
                    // checks and flags error if photo is not in landscape mode
                    if (image.PropertyIdList.Contains(0x0112))
                    {
                        int rotationValue = image.GetPropertyItem(0x0112).Value[0];
                        if (rotationValue != 1)
                        {
                            formatError = true;
                        }
                    }  
                }

                if (!formatError)
                {
                    blobUrl = await this._photoService.AddPhotoBlob(uploadedPhoto, horseName, modifiedDate);

                    if (!String.IsNullOrEmpty(blobUrl)) // photo saved successfully
                    {
                        if (oldImageUrl != null)
                        {
                            await this._photoService.RemovePhotoBlob(oldImageUrl);
                        }
                    }
                } 
 
            }
            catch (Exception e)
            {
                blobUrl = null;
            }

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
