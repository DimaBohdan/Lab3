using System;
namespace Lab3
{
    internal class Utils
    {
        public static int[] GetVariantDigits(int seed)
        {
            int digitsLength = 0;
            int tempSeed = seed;
            while (tempSeed != 0)
            {
                digitsLength++;
                tempSeed /= 10;
            }

            int[] digits = new int[digitsLength + 1];
            digits[0] = seed;
            for (int i = digitsLength; i > 0; i--)
            {
                digits[i] = seed % 10;
                seed /= 10;
            }
            return digits;
        }
    }
}
