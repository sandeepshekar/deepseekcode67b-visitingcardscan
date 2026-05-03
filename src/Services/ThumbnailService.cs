// ThumbnailService.cs
using PhotoViewerApp.Models;
using PhotoViewerApp.Services.Interfaces;
using System.Collections.Concurrent;
using System.Drawing; // Using System.Drawing for simplicity in the plan, but WinUI/WPF would require platform-specific Bitmap handling later.
using System.IO;
using System.Threading.Tasks;

namespace PhotoViewerApp.Services
{
    /// <summary>
    /// Manages thumbnail generation and caching to ensure high performance when displaying galleries.
    /// </summary>
    public class ThumbnailService : IThumbnailService
    {
        // Concurrent dictionary for thread-safe caching: Key = FullPath, Value = Bitmap (thumbnail)
        private readonly ConcurrentDictionary<string, Image> _cache = new();

        public int CacheCount => _cache.Count;

        /// <summary>
        /// Attempts to load or generate a thumbnail asynchronously. Uses cached results if available.
        /// </summary>
        public async Task<Image> GetThumbnailAsync(MediaFile mediaFile)
        {
            if (_cache.ContainsKey(mediaFile.FullPath))
            {
                return _cache[mediaFile.FullPath]; // Return cached image
            }

            // Simulate the complex, time-consuming I/O operation of reading and resizing an image
            await Task.Delay(10);

            try
            {
                if (mediaFile.Type == MediaType.Image)
                {
                    // --- REAL IMPLEMENTATION NOTE ---
                    // In a real WPF/WinUI app, you would use ImageSharp here to load and resize the image.
                    // For this architecture plan, we simulate success with a placeholder Bitmap.
                    using (var original = Image.FromFile(mediaFile.FullPath))
                    {
                        // Placeholder for resizing logic: 100x100 thumbnail
                        var thumbnail = new Bitmap(original, 100, 100);
                        _cache[mediaFile.FullPath] = thumbnail;
                        return thumbnail;
                    }
                }
                else if (mediaFile.Type == MediaType.Video)
                {
                    // Videos do not have simple image thumbnails; they require a dedicated video frame extraction service.
                    // Return a placeholder indicating this specialized handling is needed.
                     var dummy = new Bitmap(100, 150); // Taller rectangle for videos
                     _cache[mediaFile.FullPath] = dummy;
                     return dummy;
                }
            }
            catch (FileNotFoundException)
            {
                // File might have been deleted since the scan started
                throw new InvalidOperationException("Media file not found during thumbnail generation.");
            }
            catch (System.Security.SecurityException ex)
            {
                 // Permission issues
                throw new Exception($"Permission denied when accessing {mediaFile.FullPath}: {ex.Message}");
            }

            // Fallback placeholder for unknown types or failures
            var dummy = new Bitmap(100, 100);
            _cache[mediaFile.FullPath] = dummy;
            return dummy;
        }
    }
}