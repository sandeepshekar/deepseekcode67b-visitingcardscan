using Xunit;
using Moq;
using System.IO;
using PhotoViewerApp.Services;
using PhotoViewerApp.Models;

namespace PhotoViewerApp.Tests.Services
{
    public class FileScanServiceTests
    {
        // Note: For actual testing, we would use a virtual file system or create temporary directories
        // and clean them up after each test run to avoid relying on the machine's actual filesystem state.

        [Fact]
        public async Task ScanDirectoryAsync_WhenGivenValidPath_ShouldReturnSupportedMediaFiles()
        {
            // Arrange: Setup a mock environment (or temporary directory) with mixed files.
            string tempPath = Path.Combine(Path.GetTempPath(), "TestScanDir");
            Directory.CreateDirectory(tempPath);

            // Create dummy files for testing
            File.WriteAllText(Path.Combine(tempPath, "photo1.jpg"), "dummy jpg content");
            File.WriteAllText(Path.Combine(tempPath, "video1.mp4"), "dummy mp4 content");
            File.WriteAllText(Path.Combine(tempPath, "document.txt"), "This should be ignored.");

            var service = new FileScanService();

            // Act
            var result = await service.ScanDirectoryAsync(tempPath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Should only find JPG and MP4

            // Cleanup: Remove dummy files/directories after test completion
            try { Directory.Delete(tempPath, true); } catch (Exception ex) { /* Ignore cleanup errors */ }
        }

        [Fact]
        public async Task ScanDirectoryAsync_WhenGivenInvalidPath_ShouldThrowDirectoryNotFoundException()
        {
            // Arrange: A path guaranteed not to exist
            string invalidPath = Path.Combine(Path.GetTempPath(), "NonExistentTestDirForFailure");

            var service = new FileScanService();

            // Act & Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => service.ScanDirectoryAsync(invalidPath));
        }
    }
}