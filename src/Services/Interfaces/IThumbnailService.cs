// ITumbnailService.cs
using PhotoViewerApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoViewerApp.Services.Interfaces
{
    /// <summary>
    /// Contract for handling the efficient loading and caching of media thumbnails.
    /// </summary>
    public interface IThumbnailService
    {
        /// <summary>
        /// Generates or retrieves a cached thumbnail bitmap (or placeholder) for a given file path.
        /// This must be asynchronous and non-blocking to ensure UI responsiveness.
        /// </summary>
        Task<System.Drawing.Image> GetThumbnailAsync(MediaFile mediaFile);

        /// <summary>
        /// Gets the cache size count (for monitoring purposes).
        /// </summary>
        int CacheCount { get; }
    }
}