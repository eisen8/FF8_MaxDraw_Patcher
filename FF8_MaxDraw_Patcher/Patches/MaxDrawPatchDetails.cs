using FF8_MaxDraw_Patcher.Utils;

namespace FF8_MaxDraw_Patcher.Patch
{
    /// <summary>
    /// The FF8 Max Draw Patch Details. Changing 4 bytes is all we need. 
    /// </summary>
    public class MaxDrawPatchDetails : IPatch
    {
        /// <summary>
        /// The bytes we are inserting.
        /// </summary>
        public byte[] Patch { get; } = ByteUtil.FromHexString("33 C0 04 64");

        /// <summary>
        /// The original bytes we are replacing (used to find where to patch).
        /// </summary>
        public byte[] Unpatch { get; } = ByteUtil.FromHexString("79 17 33 C0");

        /// <summary>
        /// The 16 Bytes before the unpatch bytes to validate the file version. Necessary because the 4 bytes aren't unique enough.
        /// </summary>
        public byte[] PreValidationBits { get; } = ByteUtil.FromHexString("FA 8B C2 C1 E8 1F 03 D0 8B C2 2B C7 5F 5E 5D 5B");

        /// <summary>
        /// The 16 Bytes after the unpatch bytes to validate the file version.
        /// </summary>
        public byte[] PostValidationBits { get; } = ByteUtil.FromHexString("C3 8B 4C 24 18 8D 14 8A 33 C9 8A 8C 53 05 01 00");
    }
}
