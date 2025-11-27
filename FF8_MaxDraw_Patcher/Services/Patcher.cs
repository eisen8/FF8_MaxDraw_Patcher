using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Utils;
using System;
using System.IO;
using System.Linq;

namespace FF8_MaxDraw_Patcher.Services
{
    public class Patcher
    {
        private readonly MaxDrawPatchDetails _patch;
        private readonly Logger _l;

        public Patcher(MaxDrawPatchDetails patch, Logger logger)
        {
            _patch = patch;
            _l = logger;
        }

        public string BackupFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //Log("Cannot backup: file does not exist.");
                return "";
            }

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                string filename = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);

                // Create a backup filename with timestamp
                string backupFile = Path.Combine(directory, $"{filename}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}.bak");

                File.Copy(filePath, backupFile, overwrite: false);
                //Log($"Backup created: {backupFile}");
                return backupFile;
            }
            catch (Exception ex)
            {
                //Log($"Failed to create backup: {ex.Message}");
                return "";
            }
        }

        public int SearchBytePattern(string filePath, byte[] pattern)
        {
            if (!File.Exists(filePath))
            {
                //Log("File does not exist.");
                return -1;
            }

            byte[] data = File.ReadAllBytes(filePath);

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

        public bool Patch(string filePath)
        {
            Byte[] searchPattern = _patch.PreValidationBits.Concat(_patch.Unpatch).Concat(_patch.PostValidationBits).ToArray();

            long offset = SearchBytePattern(filePath, searchPattern);
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

        public bool IsFilePatchable(string filePath)
        {
            Byte[] searchPattern = ByteUtil.ConcatArrays(_patch.PreValidationBits, _patch.Unpatch, _patch.PostValidationBits);
            return SearchBytePattern(filePath, searchPattern) != -1;
        }

        public bool IsFileAlreadyPatched(string filePath)
        {
            Byte[] searchPattern = ByteUtil.ConcatArrays(_patch.PreValidationBits, _patch.Patch, _patch.PostValidationBits);
            return SearchBytePattern(filePath, searchPattern) != -1;
        }
    }
}
