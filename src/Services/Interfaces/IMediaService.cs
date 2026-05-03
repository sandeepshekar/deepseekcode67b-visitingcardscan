// IMediaService.cs
using PhotoViewerApp.Models;
using System.Threading.Tasks;

namespace PhotoViewerApp.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for handling media playback and presentation logic based on media type.
    /// </summary>
    public interface IMediaService
    {
        /// <summary>
        /// Loads the selected media file into a format suitable for display (bitmap, video player control, etc.).
        /// This method should be highly decoupled and abstract away the specific library calls (ImageSharp vs VLC).
        /// </summary>
        /// <param name="mediaFile">The metadata of the selected media item.</param>
        /// <returns>A Task representing the loading/playback preparation, potentially returning a generic display object.</returns>
        Task<object> DisplayMediaContentAsync(MediaFile mediaFile);
    }

    // Helper class to hold the resulting content structure in the View Model
    public class MediaDisplayResult
    {
        public MediaType Type { get; set; }
        public object Content { get; set; } // Could be Bitmap for images, or a specialized VideoPlayerControl instance
    }
}