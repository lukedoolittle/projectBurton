using System;
using System.Collections.Generic;
using System.Text;

namespace Burton.Core.Common
{
    public static class RandomNumberGenerator
    {
        private static readonly Random RANDOM_NUMBER_GENERATOR =
            new Random();

        public static bool RandomBoolean()
        {
            return RANDOM_NUMBER_GENERATOR.Next(2) == 0;
        }

        public static int RandomNumber(int maximum)
        {
            return RANDOM_NUMBER_GENERATOR.Next(maximum);
        }
    }
}
