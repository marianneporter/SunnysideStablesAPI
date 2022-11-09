using System.Collections.Generic;

namespace SunnysideStablesAPI.Models
{
    public class HorseListData
    {
        public int CountAll { get; set; }
        public int SearchCount { get; set; }
        public List<Horse> Horses { get; set; }   
    }
}
