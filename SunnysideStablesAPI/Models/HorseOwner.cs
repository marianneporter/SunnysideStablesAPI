using SunnysideStablesAPI.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Models
{
    public class HorseOwner
    {
        public int HorseId { get; set; }         
        public int OwnerId { get; set; }
        public Horse Horse { get; set; }      
        public Owner Owner { get; set; }
    }
}
