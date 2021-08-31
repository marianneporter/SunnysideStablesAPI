using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.UtilityServices.PhotoBlobs
{
    public interface IPhotoBlobService
    {
        Task RemovePhotoBlob(string oldImageUrl);
        Task<string> AddPhotoBlob(IFormFile uploadedPhoto, string horseName, DateTime modifiedDate);

    }
}
