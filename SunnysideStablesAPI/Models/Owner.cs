using SunnysideStablesAPI.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Models
{
    public class Owner : User
    {      
        public ICollection<HorseOwner> HorseOwner { get; set; }
    }
}
