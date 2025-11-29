using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Services.Interfaces;
using FF8_MaxDraw_Patcher.Utils;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;


namespace FF8_MaxDraw_Patcher.Services
{
    /// <summary>
    /// A class for validating files before patching.
    /// </summary>
    public class FileValidator : IFileValidator
    {
        private readonly IPatcher _patcher;
        private readonly Logger _l;
        private readonly IFileSystem _fs;

        public FileValidator(IPatcher patcher, IFileSystem fileSystem, Logger logger)
        {
            _patcher = patcher;
            _l = logger;
            _fs = fileSystem;
        }

        /// <summary>
        /// Validates the file.
        /// </summary>
        /// <param name="filePath">The file to validate.</param>
        /// <returns>The validation result.</returns>
        public async Task<FileValidationResult> ValidateFile(string filePath)
        {
            if (filePath == null || string.IsNullOrWhiteSpace(filePath))
            {
                return new FileValidationResult() { Success = false, Message = "No file selected." };
            }

            // Validate the file exists
            if (!_fs.File.Exists(filePath))
            {
                return new FileValidationResult() { Success = false, Message = "Selected file does not exist." };
            }

            // Validate the extension is correct
            if (!_fs.Path.GetExtension(filePath).ToLower().Equals(".exe"))
            {
                return new FileValidationResult() { Success = false, Message = "Selected file must be an executable (.exe)." };
            }

            // Validate approximate file size
            IFileInfo fi = _fs.FileInfo.New(filePath);
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
        private bool IsFileWritable(string filePath)
        {
            try
            {
                using var stream = _fs.File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
                // If we can open it for read/write, it's writable
                return true;
            }
            catch { return false; }
        }
    }
}
