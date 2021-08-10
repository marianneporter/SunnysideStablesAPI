using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SunnysideStablesAPI.Data.Repository;
using SunnysideStablesAPI.Dtos;
using SunnysideStablesAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IConfiguration _config;

        public HorsesController(IStablesRepo repo,
                                IMapper mapper,
                                IConfiguration config)
        {
            _repo = repo;
            _mapper = mapper;
            _config = config;
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
                var photoUrl = await SavePhoto(horseAddUpdateDto.ImageFile, horseToAdd.Id, horseToAdd.Name);

                if (!String.IsNullOrEmpty(photoUrl)) // photo saved successfully
                {
                    horseToAdd.ImageUrl = photoUrl;
                }
            }

            await _repo.Commit();

            return Created("~api/horses", new { id = horseToAdd.Id, name = horseToAdd.Name });

        }


        private async Task<string> SavePhoto(IFormFile uploadedPhoto, int id, string horseName)
        {

            var filename = horseName.Replace(" ", String.Empty).ToLower() + '_' + id.ToString() + ".jpg";

            var azureConnectionString = _config.GetConnectionString("AzureStorageConnection");
            var azureContainer = _config.GetValue<string>("StorageContainer");

            var container = new BlobContainerClient(azureConnectionString, azureContainer);
            var blob = container.GetBlobClient(filename);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

            using (var stream = uploadedPhoto.OpenReadStream())
            {
                using (var output = new MemoryStream())
                using (Image image = Image.Load(stream))
                {
                    image.Mutate(x => x.Resize(600, 450));
                    image.SaveAsJpeg(output);
                    output.Position = 0;
                    await blob.UploadAsync(output, new BlobHttpHeaders { ContentType = "image/jpeg" });
                }
            }


            var blobUrl = blob.Uri.AbsoluteUri;

            return blobUrl;
        }
    }
}
