using SunnysideStablesAPI.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Models
{
    public class Staff : User
    {
        public string JobTitle { get; set; }
    }
}
