using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SunnysideStablesAPI.Dtos
{
    public class HorseAddUpdateDto : HorseDto
    {

        public ICollection<int> OwnerIds { get; set; }
        public IFormFile ImageFile { get; set; }
        public bool PhotoReset { get; set; }
    }
}
