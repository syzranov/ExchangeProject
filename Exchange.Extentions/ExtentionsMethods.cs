using System;
using System.Security.Cryptography;

namespace Exchange.Extentions
{
    public static class ExtentionsMethods
    {
        public static int ToInt(this int? value)
        {
            return Convert.ToInt32(value ?? 0);
        }
        public static decimal Next(this decimal max)
        {
            bool ok = false;
            decimal val = default(decimal);
            while (!ok)
            {
                // Create a int array to hold the random values.
                Byte[] bytes = new Byte[] { 0, 0, 0, 0 };

                RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();

                // Fill the array with a random value.
                Gen.GetBytes(bytes);
                bytes[3] %= 29; // this must be between 0 and 28 (inclusive)
                decimal d = new decimal((int)bytes[0], (int)bytes[1], (int)bytes[2], false, bytes[3]);

                var temp = decimal.Round(d % (max + 1), 2);
                if (temp > 0 && temp < max)
                {
                    ok = true;
                    val = temp;
                }
            }

            return val;
        }
        
    }
}
