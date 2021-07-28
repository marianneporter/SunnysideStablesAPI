using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Dtos
{
    public class HorseAddUpdateDto : HorseDto
    {
        public ICollection<int> OwnerIds { get; set; }
        public IFormFile PhotoFile { get; set; }
    }
}
