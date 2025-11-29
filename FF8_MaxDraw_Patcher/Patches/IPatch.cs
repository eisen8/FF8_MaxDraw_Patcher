using FF8_MaxDraw_Patcher.Utils;

namespace FF8_MaxDraw_Patcher.Patch
{
    public interface IPatch
    {
        /// <summary>
        /// Bytes to insert.
        /// </summary>
        byte[] PatchBytes { get; }

        /// <summary>
        /// Original bytes to be replaced by the PatchBytes.
        /// </summary>
        byte[] OriginalBytes { get; }

        /// <summary>
        /// Validation bytes before the unpatch. Necessary if the patch bytes aren't long and unique enough.
        /// </summary>
        byte[] PreValidationBytes { get; }

        /// <summary>
        /// Validation bytes after the unpatch.  Necessary if the patch bytes aren't long and unique enough.
        /// </summary>
        byte[] PostValidationBytes { get; }
    }
}