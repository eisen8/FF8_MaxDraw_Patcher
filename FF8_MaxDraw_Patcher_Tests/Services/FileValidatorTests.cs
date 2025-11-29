using FF8_MaxDraw_Patcher.Services;
using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Utils;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using FF8_MaxDraw_Patcher.Services.Interfaces;

namespace FF8_MaxDraw_Patcher_Tests
{
    public class FileValidatorTests
    {
        private readonly MockFileSystem _fs;
        private readonly IPatcher _patcher;
        private readonly Logger _logger;
        private readonly FileValidator _validator;
        private readonly string _filePath = @"C:\FF8.exe";

        public FileValidatorTests()
        {
            _fs = new MockFileSystem();
            _patcher = Substitute.For<IPatcher>();
            _logger = Substitute.For<Logger>();

            _validator = new FileValidator(_patcher, _fs, _logger);
        }

        [Fact]
        public async Task ValidateFile_NullFilePath_ReturnsFailure()
        {
            // Act
            var result = await _validator.ValidateFile(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No file selected.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_EmptyFilePath_ReturnsFailure()
        {
            // Act
            var result = await _validator.ValidateFile("");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No file selected.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_WhitespaceFilePath_ReturnsFailure()
        {
            // Act
            var result = await _validator.ValidateFile("     ");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No file selected.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_FileDoesNotExist_ReturnsFailure()
        {
            // Act
            var result = await _validator.ValidateFile(_filePath);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Selected file does not exist.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_InvalidExtension_ReturnsFailure()
        {
            _fs.AddFile(@"C:\FF8.txt", new MockFileData(new byte[20000000]));

            // Act
            var result = await _validator.ValidateFile(@"C:\FF8.txt");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Selected file must be an executable (.exe).", result.Message);
        }

        [Theory]
        [InlineData(5_000_000)]
        [InlineData(50_000_000)]
        public async Task ValidateFile_InvalidSize_ReturnsFailure(long size)
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[size]));

            // Act
            var result = await _validator.ValidateFile(_filePath);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("File size is invalid for FF8.exe.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_NotWritable_ReturnsFailure()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[20000000]));
            _fs.File.SetAttributes(_filePath, System.IO.FileAttributes.ReadOnly);

            // Act
            var result = await _validator.ValidateFile(_filePath);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot write to file. Check permissions or run as admin.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_NotPatchableButAlreadyPatched_ReturnsFailure()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[20000000]));
            _patcher.IsFilePatchable(_filePath).Returns(Task.FromResult(false));
            _patcher.IsFileAlreadyPatched(_filePath).Returns(Task.FromResult(true));

            // Act
            var result = await _validator.ValidateFile(_filePath);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("File is already patched.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_NotPatchableAndNotAlreadyPatched_ReturnsFailure()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[20000000]));
            _patcher.IsFilePatchable(_filePath).Returns(Task.FromResult(false));
            _patcher.IsFileAlreadyPatched(_filePath).Returns(Task.FromResult(false));

            // Act
            var result = await _validator.ValidateFile(_filePath);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("File is not patchable. Incompatible version or incorrect file.", result.Message);
        }

        [Fact]
        public async Task ValidateFile_ValidFile_ReturnsSuccess()
        {
            _fs.AddFile(_filePath, new MockFileData(new byte[20000000]));
            _patcher.IsFilePatchable(_filePath).Returns(Task.FromResult(true));

            // Act
            var result = await _validator.ValidateFile(_filePath);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("File is patchable.", result.Message);
        }
    }
}