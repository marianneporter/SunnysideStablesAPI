using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnysideStablesAPI.Utility
{
    public static class Utility
    {
        public static double HandsToCm(string heightHands)
        {
            string[] parts = heightHands.Trim().Split('.');
            var heightInches = (Int32.Parse(parts[0]) * 4) + Int32.Parse(parts[1]);
            return heightInches * 2.54;
        }
    }
}
