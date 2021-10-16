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
