using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Utils;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FF8_MaxDraw_Patcher.Services
{
    /// <summary>
    /// Provides patching services for applying patches to files.
    /// </summary>
    public class Patcher
    {
        private readonly IPatch _patch;
        private readonly Logger _l;

        public Patcher(IPatch patch, Logger logger)
        {
            _patch = patch;
            _l = logger;
        }

        /// <summary>
        /// Backups the specified file by creating a copy with a .bak extension.
        /// </summary>
        /// <param name="filePath">The file to backup</param>
        /// <returns>The full filepath to the backup file. Returns an empty string if the file cannot be backed up.</returns>
        public string BackupFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _l.Log("Cannot backup file: File does not exist.");
                return "";
            }

            try
            {
                string directory = Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory();
                string filename = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);

                // Create a backup filename with timestamp and .bak extension
                string backupFile = Path.Combine(directory, $"{filename}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}.bak");

                File.Copy(filePath, backupFile, overwrite: false);

                return backupFile;
            }
            catch (Exception e)
            {
                _l.LogError(e, "Error creating backup file.");
                return "";
            }
        }


        /// <summary>
        /// Patches the filePath.
        /// </summary>
        /// <param name="filePath">The file to patch.</param>
        /// <returns>True if the patch was successful or false if not.</returns>
        public async Task<bool> Patch(string filePath)
        {
            Byte[] searchPattern = ByteUtil.ConcatArrays(_patch.PreValidationBits, _patch.Unpatch, _patch.PostValidationBits);
            long offset = await SearchBytePattern(filePath, searchPattern);

            if (offset != -1)
            {
                offset += _patch.PreValidationBits.Length;
                using FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Write);
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Write(_patch.Patch, 0, _patch.Patch.Length);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a file is patchable..
        /// </summary>
        /// <param name="filePath">The file to check.</param>
        /// <returns>True if patchable or false if not.</returns>
        public async Task<bool> IsFilePatchable(string filePath)
        {
            Byte[] searchPattern = ByteUtil.ConcatArrays(_patch.PreValidationBits, _patch.Unpatch, _patch.PostValidationBits);
            return await SearchBytePattern(filePath, searchPattern) != -1;
        }

        /// <summary>
        /// Checks if a file is already patched...
        /// </summary>
        /// <param name="filePath">The file to check.</param>
        /// <returns>True if the file is already patched or false if not.</returns>
        public async Task<bool> IsFileAlreadyPatched(string filePath)
        {
            Byte[] searchPattern = ByteUtil.ConcatArrays(_patch.PreValidationBits, _patch.Patch, _patch.PostValidationBits);
            return await SearchBytePattern(filePath, searchPattern) != -1;
        }


        /// <summary>
        /// Searches for a byte pattern within a file and returns the offset of its first occurrence.
        /// </summary>
        /// <param name="filePath">The filepath</param>
        /// <param name="pattern">The byte pattern.</param>
        /// <returns>The offset of the first occurence or -1 if not found.</returns>
        private async Task<int> SearchBytePattern(string filePath, byte[] pattern)
        {
            if (!File.Exists(filePath))
            {
                _l.Log("Cannot Search File: File does not exist.");
                return -1;
            }

            // We could read in sections and do a faster algorithm (like KMP or Boyer-Moore) but the patch file is small and is read very quickly (< 1s).

            byte[] data = await File.ReadAllBytesAsync(filePath);

            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool match = true;

                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
