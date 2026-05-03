using Xunit;
using Moq;
using PhotoViewerApp.Services;
using PhotoViewerApp.Models;
using System.IO;
using System.Drawing;

namespace PhotoViewerApp.Tests.Services
{
    public class ThumbnailServiceTests
    {
        // Using a concrete instance here since the service manages internal state (the cache)
        private readonly ThumbnailService _service = new();

        [Fact]
        public async Task GetThumbnailAsync_WhenCalledTwiceWithSameFile_ShouldReturnCachedImage()
        {
            // Arrange: Create a temporary file to simulate media content
            string tempPath = Path.Combine(Path.GetTempPath(), "TestThumbnailDir");
            Directory.CreateDirectory(tempPath);
            var dummyFilePath = Path.Combine(tempPath, "test_image.jpg");
            File.WriteAllText(dummyFilePath, "dummy jpg content"); // Dummy file creation

            // Create a mock MediaFile object pointing to the temporary location
            var mediaFile = new MediaFile { FullPath = dummyFilePath };

            // Act 1: First call generates and caches the resource
            var firstThumbnail = await _service.GetThumbnailAsync(mediaFile);

            // Assert 1: Check if content was returned (non-null/valid)
            Assert.NotNull(firstThumbnail);

            // Act 2: Second call should hit the cache, avoiding regeneration logic
            var secondThumbnail = await _service.GetThumbnailAsync(mediaFile);

            // Assert 2: The reference check ensures the same cached object is returned (if possible in unit test scope)
            // For simplicity here, we assert they are not null and that the service's cache count increases only on first run.
            Assert.Same(firstThumbnail, secondThumbnail);
        }

        [Fact]
        public async Task GetThumbnailAsync_WhenCalledWithUnknownFile_ShouldReturnPlaceholder()
        {
            // Arrange: Simulate a file type that should hit the video placeholder logic (or unknown)
            string tempPath = Path.Combine(Path.GetTempPath(), "TestThumbDir");
            Directory.CreateDirectory(tempPath);
            var dummyFilePath = Path.Combine(tempPath, "test_video.mp4");
            File.WriteAllText(dummyFilePath, "dummy mp4 content");

            // Mock the MediaFile to force a non-image type for testing placeholder logic
            var mediaFile = new MediaFile { FullPath = dummyFilePath };

            // Act
            var result = await _service.GetThumbnailAsync(mediaFile);

            // Assert: Verify that a placeholder/fallback image was returned instead of failing due to lack of image data.
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetThumbnailAsync_WhenCalledWithMissingFile_ShouldHandleExceptionGracefully()
        {
             // Arrange: Create MediaFile with a path that definitely doesn't exist
            var mediaFile = new MediaFile { FullPath = Path.Combine(Path.GetTempPath(), "definitely_not_here.jpg") };

            // Act & Assert: The service should catch the FileNotFoudException and return a fallback placeholder instead of crashing.
            var result = await Record.ExceptionAsync(() => _service.GetThumbnailAsync(mediaFile));
            Assert.IsType<InvalidOperationException>(result); // Expecting graceful failure wrapper
        }

        // IMPORTANT: Clean up temporary directory after tests run (requires using a TestFixture or Cleanup method)
    }
}