using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Services;
using FF8_MaxDraw_Patcher.Services.Interfaces;
using FF8_MaxDraw_Patcher.Utils;
using NSubstitute;
using System;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Xunit;

namespace FF8_MaxDraw_Patcher_Tests
{
    public class PatcherTests
    {
        private readonly MockFileSystem _fs;
        private readonly IPatch _patch;
        private readonly Logger _logger;
        private readonly Patcher _patcher;
        private readonly string _filePath = @"C:\FF8.exe";

        public PatcherTests()
        {
            _fs = new MockFileSystem();
            _patch = new MaxDrawPatchDetails();
            _logger = Substitute.For<Logger>();

            _patcher = new Patcher(_patch, _fs, _logger);
        }

        [Fact]
        public void BackupFile_FileDoesNotExist_ReturnsEmpty_AndLogs()
        {
            // Act
            string result = _patcher.BackupFile(_filePath);

            // Assert
            Assert.Equal(string.Empty, result);
            _logger.Received(1).Log(Arg.Is<string>(s => s.Contains("Cannot backup file")));
        }

        [Fact]
        public void BackupFile_FileExists_ReturnsBackupPath()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[10]));

            // Act
            string backupPath = _patcher.BackupFile(_filePath);

            // Assert
            Assert.False(string.IsNullOrEmpty(backupPath));
            Assert.True(_fs.FileExists(backupPath));
        }

        [Fact]
        public void BackupFile_MultipleBackups()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[10]));

            // Act
            string backup1 = _patcher.BackupFile(_filePath);
            string backup2 = _patcher.BackupFile(_filePath);
            string backup3 = _patcher.BackupFile(_filePath);

            // Assert
            Assert.Equal(@"C:\FF8.exe.bak", backup1);
            Assert.True(_fs.FileExists(backup1));

            Assert.Equal(@"C:\FF8_2.exe.bak", backup2);
            Assert.True(_fs.FileExists(backup2));

            Assert.Equal(@"C:\FF8_3.exe.bak", backup3);
            Assert.True(_fs.FileExists(backup3));
        }

        [Fact]
        public async Task Patch_FileContainsPattern_WritesPatch_ReturnsTrue()
        {
            byte[] data = returnUnpatchedFile();
            _fs.AddFile(_filePath, new MockFileData(data));
            byte[] expectedData = returnPatchedFile();

            // Act
            bool result = await _patcher.Patch(_filePath);

            // Assert
            byte[] patchedData = await _fs.File.ReadAllBytesAsync(_filePath);
            Assert.True(result);
            Assert.Equal(expectedData, patchedData);
        }

        [Fact]
        public async Task Patch_FileDoesNotContainPattern_ReturnsFalse()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[20]));

            // Act
            bool result = await _patcher.Patch(_filePath);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Patch_FileAlreadyPatched_ReturnsFalse()
        {
            byte[] data = returnPatchedFile();
            _fs.AddFile(_filePath, new MockFileData(data));

            // Act
            bool result = await _patcher.Patch(_filePath);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public async Task IsFilePatchable_FileContainsPattern_ReturnsTrue()
        {
            byte[] data = returnUnpatchedFile();
            _fs.AddFile(_filePath, new MockFileData(data));

            // Act
            bool result = await _patcher.IsFilePatchable(_filePath);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsFilePatchable_FileDoesNotContainPattern_ReturnsFalse()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[10]));

            // Act
            bool result = await _patcher.IsFilePatchable(_filePath);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsFilePatchable_FileContainsPatch_ReturnsFalse()
        {
            byte[] data = returnPatchedFile();
            _fs.AddFile(_filePath, new MockFileData(data));

            // Act
            bool result = await _patcher.IsFilePatchable(_filePath);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public async Task IsFileAlreadyPatched_FileContainsPatch_ReturnsTrue()
        {
            byte[] data = returnPatchedFile();
            _fs.AddFile(_filePath, new MockFileData(data));

            // Act
            bool result = await _patcher.IsFileAlreadyPatched(_filePath);


            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsFileAlreadyPatched_FileIsUnpatched_ReturnsFalse()
        {
            byte[] data = returnUnpatchedFile();
            _fs.AddFile(_filePath, new MockFileData(data));

            // Act
            bool result = await _patcher.IsFileAlreadyPatched(_filePath);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsFileAlreadyPatched_FileDoesNotContainPattern_ReturnsFalse()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[10]));

            // Act
            bool result = await _patcher.IsFileAlreadyPatched(_filePath);

            // Assert
            Assert.False(result);
        }


        private byte[] returnPatchedFile()
        {
            return ByteUtil.ConcatArrays(_patch.PreValidationBytes, _patch.PatchBytes, _patch.PostValidationBytes);
        }

        private byte[] returnUnpatchedFile()
        {
            return ByteUtil.ConcatArrays(_patch.PreValidationBytes, _patch.OriginalBytes, _patch.PostValidationBytes);
        }
    }
}