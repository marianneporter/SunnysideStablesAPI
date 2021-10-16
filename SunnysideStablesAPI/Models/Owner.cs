using SunnysideStablesAPI.Models.Identity;
using System.Collections.Generic;

namespace SunnysideStablesAPI.Models
{
    public class Owner : User
    {      
        public ICollection<HorseOwner> HorseOwner { get; set; }
    }
}
