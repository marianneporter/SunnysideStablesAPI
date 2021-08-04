using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnysideStablesAPI.Models
{
    public class Horse
    {
        public Horse()
        {
            this.CreatedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Sex  { get; set; }
        public string Colour { get; set; }
        public double Heightcm { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<HorseOwner> HorseOwner { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [NotMapped]
        public string HeightHands
        {
            get
            {
                var heightInInches = Math.Round(Heightcm * 0.3937007874);
                var wholeHands = Math.Truncate(heightInInches / 4);
                var remainingInches = heightInInches - (wholeHands * 4);
                return $"{wholeHands}.{remainingInches}";
            }

        }
    }
}
