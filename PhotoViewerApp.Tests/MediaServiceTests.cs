using Xunit;
using Moq;
using PhotoViewerApp.Services;
using PhotoViewerApp.Models;
using System.Drawing;

namespace PhotoViewerApp.Tests.Services
{
    public class MediaServiceTests
    {
        // Note: In a real unit test, we would mock IThumbnailService fully.
        // We are using a simplified structure here to focus on the logic flow of MediaService.
        [Fact]
        public async Task DisplayMediaContentAsync_WhenInputIsImage_ShouldUseImageLoadingPath()
        {
            // Arrange: Setup mock dependencies (for Image)
            var mockThumbnailService = new Mock<IThumbnailService>();
            // Provide a dummy result for the thumbnail service to return
            mockThumbnailService.Setup(s => s.GetThumbnailAsync(It.IsAny<MediaFile>()))
                               .ReturnsAsync(new Bitmap(100, 100));

            var service = new MediaService(mockThumbnailService.Object);

            // Setup mock media file
            var imageFile = new MediaFile { FullPath = "path/to/image.jpg" };

            // Act
            var result = await service.DisplayMediaContentAsync(imageFile);

            // Assert: Check if the result is marked as image content (using placeholder strings)
            Assert.IsType<string>(result);
            Assert.Contains("IMAGE_DISPLAY", (string)result);
        }

        [Fact]
        public async Task DisplayMediaContentAsync_WhenInputIsVideo_ShouldUseVideoLoadingPath()
        {
            // Arrange: Setup mock dependencies (Image service is less critical here, but must exist)
            var mockThumbnailService = new Mock<IThumbnailService>();
            mockThumbnailService.Setup(s => s.GetThumbnailAsync(It.IsAny<MediaFile>()))
                               .ReturnsAsync(new Bitmap(100, 150)); // Placeholder for video thumbnail

            var service = new MediaService(mockThumbnailService.Object);

            // Setup mock media file
            var videoFile = new MediaFile { FullPath = "path/to/video.mp4" };

            // Act
            var result = await service.DisplayMediaContentAsync(videoFile);

            // Assert: Check if the result is marked as video content
            Assert.IsType<string>(result);
            Assert.Contains("VIDEO_DISPLAY", (string)result);
        }

         [Fact]
        public async Task DisplayMediaContentAsync_WhenInputIsUnknownType_ShouldReturnFallbackMessage()
        {
            // Arrange: Setup mock dependencies
            var mockThumbnailService = new Mock<IThumbnailService>();
             mockThumbnailService.Setup(s => s.GetThumbnailAsync(It.IsAny<MediaFile>()))
                               .ReturnsAsync(new Bitmap(100, 100));

            var service = new MediaService(mockThumbnailService.Object);

            // Setup mock media file with an unsupported extension (e.g., .zip)
            var unknownFile = new MediaFile { FullPath = "path/to/archive.zip" };

            // Act
            var result = await service.DisplayMediaContentAsync(unknownFile);

            // Assert: Check if the fallback message was returned
            Assert.IsType<string>(result);
            Assert.Contains("Unsupported media type", (string)result);
        }
    }
}