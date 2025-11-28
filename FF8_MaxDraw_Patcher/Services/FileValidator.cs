using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Utils;
using System;
using System.IO;
using System.Threading.Tasks;


namespace FF8_MaxDraw_Patcher.Services
{
    /// <summary>
    /// A class for validating files before patching.
    /// </summary>
    public class FileValidator
    {
        private readonly Patcher _patcher;
        private readonly Logger _l;

        public FileValidator(Patcher patcher, Logger logger)
        {
            _patcher = patcher;
            _l = logger;
        }

        /// <summary>
        /// Validates the file.
        /// </summary>
        /// <param name="filePath">The file to validate.</param>
        /// <returns>The validation result.</returns>
        public async Task<FileValidationResult> ValidateFile(string filePath)
        {
            // Validate the file exists
            if (!File.Exists(filePath))
            {
                return new FileValidationResult() { Success = false, Message = "Selected file does not exist." };
            }

            // Validate the extension is correct
            if (!Path.GetExtension(filePath).ToLower().Equals(".exe"))
            {
                return new FileValidationResult() { Success = false, Message = "Selected file must be an executable (.exe)." };
            }

            // Validate approximate file size
            FileInfo fi = new FileInfo(filePath);
            if (fi.Length > 40000000 || fi.Length < 10000000)
            {
                return new FileValidationResult() { Success = false, Message = "File size is invalid for FF8.exe." };
            }

            // Make sure the file is writable
            if (!IsFileWritable(filePath))
            {
                return new FileValidationResult() { Success = false, Message = "Cannot write to file. Check permissions or run as admin." };
            }

            // Validate the file is patchable and not already patched
            if (!await _patcher.IsFilePatchable(filePath))
            {
                if (await _patcher.IsFileAlreadyPatched(filePath))
                {
                    return new FileValidationResult() { Success = false, Message = "File is already patched." };
                }
                return new FileValidationResult() { Success = false, Message = "File is not patchable. Incompatible version or incorrect file." };
            }

            /// Passed all validations
            return new FileValidationResult() { Success = true, Message = "File is patchable." };
        }

        /// <summary>
        /// Checks if a file is writeable.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>True if writeable or false if not.</returns>
        private static bool IsFileWritable(string filePath)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
                // If we can open it for read/write, it's writable
                return true;
            }
            catch { return false; }
        }
    }
}
