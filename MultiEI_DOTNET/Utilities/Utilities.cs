// Utilities/Utilities.cs
using System;

namespace MultiEI.Utilities
{
    public static class Utilities
    {
        private static readonly Random random = new Random();

        public static string GeneratePIN()
        {
            return random.Next(100000, 999999).ToString();
        }

        
    }
}
