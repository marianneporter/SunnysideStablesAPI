using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.UtilityServices.PhotoBlobs
{
    public interface IPhotoBlobService
    {
        Task RemovePhotoBlob(string oldImageUrl);
        Task<string> AddPhotoBlob(IFormFile uploadedPhoto, string horseName, DateTime modifiedDate);

    }
}
