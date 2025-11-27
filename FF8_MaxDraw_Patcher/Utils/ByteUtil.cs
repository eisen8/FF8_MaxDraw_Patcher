using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF8_MaxDraw_Patcher.Utils
{
    public class ByteUtil
    {
        /// <summary>
        /// Converts a string of Hex characters into the corresponding byte array. Ignores whitespace and optional "0x" prefix.
        /// </summary>
        /// <param name="hex">The hex string</param>
        /// <returns>The byte array</returns>
        public static byte[] FromHexString(string hex)
        {
            string hexCleaned = string.Concat(hex.Where(c => !char.IsWhiteSpace(c))); // Remove whitespace

            if (hexCleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) // Remove 0x prefix
                hexCleaned = hex.Substring(2);

            return Convert.FromHexString(hexCleaned);
        }

        /// <summary>
        /// Concatenates any number of byte arrays (in order) into a single byte array.
        /// </summary>
        /// <param name="arrays">The arrays to concatenate (in order)</param>
        /// <returns>A new single array of the bytes</returns>
        public static byte[] ConcatArrays(params byte[][] arrays)
        {
            int totalLength = 0;

            // Compute total length
            foreach (var arr in arrays)
                totalLength += arr.Length;

            byte[] result = new byte[totalLength];

            // Copy arrays into result
            int offset = 0;
            foreach (var arr in arrays)
            {
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }

            return result;
        }
    }
}
