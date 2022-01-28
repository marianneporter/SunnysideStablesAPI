using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SunnysideStablesAPI.Shared;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.UtilityServices.PhotoBlobs
{
    public class PhotoBlobService : IPhotoBlobService
    {
        private readonly IConfiguration _config;
        public string AzureConnectionString { get; set; }
        public string AzureContainer { get; set; }
        public BlobContainerClient Container { get; set; }
        public PhotoBlobService(IConfiguration config)
        {
            _config = config;

            AzureConnectionString = _config.GetConnectionString("AzureStorageConnection");
            AzureContainer = _config.GetValue<string>("StorageContainer");
            Container = new BlobContainerClient(AzureConnectionString, AzureContainer);
        }

        public async Task RemovePhotoBlob(string oldImageUrl)
        {
            BlobClient blob = Container.GetBlobClient(oldImageUrl.Substring(oldImageUrl.LastIndexOf("/") + 1));   // blob client for old filename

            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

        }

        public async Task<string> AddPhotoBlob(IFormFile uploadedPhoto, string horseName, DateTime modifiedDate)
        {
            var newFilename = horseName.Replace(" ", String.Empty).ToLower() + "_" + modifiedDate.GetTimestamp().ToString() + ".jpg";
            BlobClient blob = Container.GetBlobClient(newFilename);   //blob client for new filename
            
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

            return blob.Uri.AbsoluteUri;
        }
    }
}
