using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Models
{
    public class HorseListData
    {
        public int ListCount { get; set; }
        public List<Horse> Horses { get; set; }   
    }
}
