// FileScanService.cs
using PhotoViewerApp.Models;
using PhotoViewerApp.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoViewerApp.Services
{
    /// <summary>
    /// Service responsible for traversing a directory and collecting media file metadata.
    /// </summary>
    public class FileScanService : IFileScanService
    {
        private readonly string[] _supportedExtensions = { ".jpg", ".jpeg", ".png", ".mp4", ".mov" };

        public async Task<IEnumerable<MediaFile>> ScanDirectoryAsync(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The specified path does not exist: {directoryPath}");
            }

            var mediaFiles = new List<MediaFile>();
            try
            {
                // Use EnumerateFiles for better memory performance on large directories
                var files = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories);

                foreach (var fullPath in files)
                {
                    string extension = Path.GetExtension(fullPath)?.ToLowerInvariant();
                    if (_supportedExtensions.Contains(extension))
                    {
                        mediaFiles.Add(new MediaFile { FullPath = fullPath });
                    }
                }

                // Simulate async work delay to mimic real IO operations
                await Task.Delay(50);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle cases where the app can't access certain subdirectories/files
                System.Diagnostics.Debug.WriteLine($"Access Denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during directory scan.", ex);
            }

            return mediaFiles;
        }
    }
}