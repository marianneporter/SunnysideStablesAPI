using System;
using System.Globalization;

namespace SunnysideStablesAPI.Shared
{
    public static class Utility
    {
        public static double HandsToCm(string heightHands)
        {
            string[] parts = heightHands.Trim().Split('.');
            var heightInches = (Int32.Parse(parts[0]) * 4) + Int32.Parse(parts[1]);
            return heightInches * 2.54;
        }

        public static long GetTimestamp( this DateTime date)
        {
            return new DateTimeOffset(date.Year,
                                      date.Month,
                                      date.Day,
                                      date.Hour,
                                      date.Minute,
                                      date.Second,
                                      TimeSpan.Zero).ToUnixTimeSeconds();
        }

        public static string ToTitleCase(this string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public static int GetCurrentUser(string userFromClaim)
        {
            if (Int32.TryParse(userFromClaim, out int currentUser)) 
            {
                return currentUser;
            }
            else
            {
                return 0;
            }
        }

    }
}
