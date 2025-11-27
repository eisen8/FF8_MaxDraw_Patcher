using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Utils;
using System;
using System.IO;


namespace FF8_MaxDraw_Patcher.Services
{
    public class FileValidator
    {
        private readonly Patcher _patcher;
        private readonly Logger _l;

        public FileValidator(Patcher patcher, Logger logger)
        {
            _patcher = patcher;
            _l = logger;
        }

        public FileValidationResult ValidateFile(string filePath)
        {
            if (Path.GetExtension(filePath).ToLower() != ".exe")
            {
                return new FileValidationResult() { Success = false, Message = "Selected file must be an executable (.exe)." };
            }

            if (!File.Exists(filePath))
            {
                return new FileValidationResult() { Success = false, Message = "Selected file does not exist." };
            }

            FileInfo fi = new FileInfo(filePath);
            if (fi.Length > 40000000 || fi.Length < 10000000)
            {
                return new FileValidationResult() { Success = false, Message = "File size is invalid for FF8.exe." };
            }

            if (!IsFileWritable(filePath))
            {
                return new FileValidationResult() { Success = false, Message = "Cannot write to file. Check permissions or run as admin." };
            }

            if (!_patcher.IsFilePatchable(filePath))
            {
                if (_patcher.IsFileAlreadyPatched(filePath))
                {
                    return new FileValidationResult() { Success = false, Message = "File is already patched." };
                }
                return new FileValidationResult() { Success = false, Message = "File is not patchable. Incompatible version or incorrect file." };
            }

            return new FileValidationResult() { Success = true, Message = "File is patchable." };
        }

        private bool IsFileWritable(string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    // If we can open it for read/write, it's writable
                }
                return true;
            }
            catch { return false; }
        }
    }
}
