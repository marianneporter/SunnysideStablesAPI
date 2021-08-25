using System;
using System.Collections.Generic;


namespace SunnysideStablesAPI.Dtos
{
    public class HorseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Sex { get; set; }
        public string Colour { get; set; }
        public double Heightcm { get; set; }
        public string HeightHands { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<OwnerDto> Owners { get; set; }
   
    }
}
