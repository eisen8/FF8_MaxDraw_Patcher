using FF8_MaxDraw_Patcher.Utils;

namespace FF8_MaxDraw_Patcher.Patch
{
    public interface IPatch
    {
        /// <summary>
        /// Bytes to insert.
        /// </summary>
        byte[] Patch { get; }

        /// <summary>
        /// Original bytes to replace.
        /// </summary>
        byte[] Unpatch { get; }

        /// <summary>
        /// Validation bytes before the unpatch. Necessary if the patch bytes aren't long and unique enough.
        /// </summary>
        byte[] PreValidationBits { get; }

        /// <summary>
        /// Validation bytes after the unpatch.  Necessary if the patch bytes aren't long and unique enough.
        /// </summary>
        byte[] PostValidationBits { get; }
    }
}