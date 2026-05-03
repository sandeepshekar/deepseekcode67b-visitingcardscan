// MediaFile.cs
using System;

namespace PhotoViewerApp.Models
{
    /// <summary>
    /// Represents metadata for a single media file (photo or video).
    /// </summary>
    public class MediaFile
    {
        public string FullPath { get; set; } = string.Empty;
        public string FileName => System.IO.Path.GetFileName(FullPath);

        // Simple enumeration to identify the type of media for proper handling in ViewModels/Services.
        public MediaType Type
        {
            get
            {
                var ext = System.IO.Path.GetExtension(FullPath)?.ToLowerInvariant();
                if (IsImageExtension(ext)) return MediaType.Image;
                if (IsVideoExtension(ext)) return MediaType.Video;
                return MediaType.Unknown;
            }
        }

        private bool IsImageExtension(string extension)
        {
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png";
        }

        private bool IsVideoExtension(string extension)
        {
            return extension == ".mp4" || extension == ".mov"; // Add more as needed
        }
    }

    public enum MediaType { Image, Video, Unknown }
}