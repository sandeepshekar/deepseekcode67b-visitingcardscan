// IFileScanService.cs
using PhotoViewerApp.Models;
using System.Collections.Generic;

namespace PhotoViewerApp.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for scanning a local directory and retrieving media file metadata.
    /// </summary>
    public interface IFileScanService
    {
        /// <summary>
        /// Scans the given directory path and returns all recognized MediaFile objects.
        /// </summary>
        /// <param name="directoryPath">The root directory to scan.</param>
        /// <returns>A list of media files found in the directory structure.</returns>
        Task<IEnumerable<MediaFile>> ScanDirectoryAsync(string directoryPath);
    }
}