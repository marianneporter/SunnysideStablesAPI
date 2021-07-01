using System;
using System.Collections.Generic;

namespace SunnysideStablesAPI.Models
{
    public class Horse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Sex  { get; set; }
        public string Colour { get; set; }
        public double Heightcm { get; set; }          
        public ICollection<HorseOwner> HorseOwner { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
