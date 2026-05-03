// MediaService.cs
using PhotoViewerApp.Models;
using PhotoViewerApp.Services.Interfaces;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace PhotoViewerApp.Services
{
    /// <summary>
    /// Coordinates media loading based on type, abstracting away specific library implementations (ImageSharp/VLC).
    /// </summary>
    public class MediaService : IMediaService
    {
        private readonly IThumbnailService _thumbnailService;

        public MediaService(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        public async Task<object> DisplayMediaContentAsync(MediaFile mediaFile)
        {
            if (mediaFile == null) return null;

            // 1. Simulate image loading using cached thumbnails as a starting point for the display object
            if (mediaFile.Type == MediaType.Image)
            {
                // In reality, we'd load high-res version here. For now, reuse thumbnail logic structure.
                var fullImage = await _thumbnailService.GetThumbnailAsync(mediaFile);
                return $"IMAGE_DISPLAY: {fullImage.ToString()}"; // Placeholder for actual Bitmap/Image object
            }

            // 2. Simulate video loading using VLC wrappers (High-level abstraction)
            else if (mediaFile.Type == MediaType.Video)
            {
                 // This would involve initializing and returning a specialized WPF/WinUI control instance
                return $"VIDEO_DISPLAY: Initializing player for {mediaFile.FileName} at path {mediaFile.FullPath}";
            }

            // Fallback for unknown types
            return "Unsupported media type.";
        }
    }
}