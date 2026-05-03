// GalleryViewModel.cs
using PhotoViewerApp.Models;
using PhotoViewerApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PhotoViewerApp.ViewModels
{
    /// <summary>
    /// ViewModel responsible for managing and exposing the media gallery data to the View.
    /// </summary>
    public class GalleryViewModel : BaseViewModel
    {
        private readonly IFileScanService _fileScanner;
        private readonly IThumbnailService _thumbnailService;
        private readonly IMediaService _mediaService; // Dependency for Phase 3

        // Collection bound to the gallery grid view
        public ObservableCollection<MediaDisplayItem> MediaItems { get; set; } = new();

        // The currently selected item, used by the Single Viewer pane. This property triggers content loading.
        public MediaFile SelectedMediaFile { get; set; }

        // The structured result passed to the Single View component
        public MediaDisplayResult SelectedMediaContent { get; private set; } = new(MediaType.Unknown, null);

        public GalleryViewModel(IFileScanService fileScanner, IThumbnailService thumbnailService, IMediaService mediaService)
        {
            _fileScanner = fileScanner;
            _thumbnailService = thumbnailService;
            _mediaService = mediaService;
        }

        /// <summary>
        /// Initiates the loading of media files from a specified directory.
        /// </summary>
        public async Task LoadGalleryAsync(string directoryPath)
        {
            MediaItems.Clear(); // Clear old items on new scan
            SelectedMediaFile = null; // Reset selection

            // Phase 1: Scan all file metadata (Fast I/O operation)
            var mediaFiles = await _fileScanner.ScanDirectoryAsync(directoryPath);

            // Use a temporary list to maintain insertion order before updating the ObservableCollection on the UI thread
            var initialMediaItems = new List<MediaDisplayItem>();
            foreach (var file in mediaFiles)
            {
                initialMediaItems.Add(new MediaDisplayItem { MediaFile = file, IsThumbnailLoaded = false });
            }

            // Update UI collection once all placeholders are ready
            MediaItems.Clear();
            foreach(var item in initialMediaItems)
            {
                 MediaItems.Add(item);
            }


            // Phase 2: Concurrently load thumbnails for all detected files (CPU/IO intensive operation)
            await Task.WhenAll(mediaFiles.Select((file, index) => LoadMediaItemAsync(file, index)));
        }

        /// <summary>
        /// Handles the logic when an item is clicked in the gallery view.
        /// </summary>
        public async Task HandleSelectionChanged(MediaFile selectedFile)
        {
            // This setter pattern ensures that setting SelectedMediaFile triggers content loading
            SelectedMediaFile = selectedFile;
        }

        private async Task LoadMediaItemAsync(MediaFile mediaFile, int index)
        {
            try
            {
                var thumbnail = await _thumbnailService.GetThumbnailAsync(mediaFile);

                // Update the specific item in the ObservableCollection (requires proper INotifyPropertyChanged setup on MediaDisplayItem)
                MediaItems[index].UpdateThumbnail(thumbnail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load thumbnail for {mediaFile.FileName}: {ex.Message}");
            }
        }

        private async Task LoadSelectedMediaAsync()
        {
            if (SelectedMediaFile == null) return;

            try
            {
                // Use the new media service to prepare display content asynchronously
                var resultObject = await _mediaService.DisplayMediaContentAsync(SelectedMediaFile);
                // Assuming a helper cast is available or we use the MediaDisplayResult structure directly
                SelectedMediaContent = (MediaDisplayResult)resultObject;
            }
            catch (Exception ex)
            {
                 System.Diagnostics.Debug.WriteLine($"Error loading selected media: {ex.Message}");
                 // Update UI state to show an error message instead of content
            }
        }
    }

    /// <summary>
    /// A wrapper model used specifically for displaying in the UI (View Model side).
    /// </summary>
    public class MediaDisplayItem : BaseViewModel
    {
        public MediaFile MediaFile { get; private set; } = new();
        private object _thumbnailContent; // Can hold Bitmap, placeholder, etc.

        // Expose the currently loaded content for binding in the Gallery View
        public object ThumbnailContent => _thumbnailContent;

        public bool IsThumbnailLoaded => _thumbnailContent != null && !_thumbnailContent.ToString().Contains("ERROR");

        public void UpdateThumbnail(object thumbnail)
        {
            _thumbnailContent = thumbnail;
            OnPropertyChanged(nameof(ThumbnailContent)); // Notify the View that the image is ready
        }
    }

    // Dummy implementation for demonstration purposes, assuming this exists elsewhere.
    public class BaseViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Dummy implementation for MediaDisplayResult structure (must be defined in the same scope or namespace)
    public class MediaDisplayResult : BaseViewModel
    {
        public MediaType Type { get; set; }
        public object Content { get; set; }

        public MediaDisplayResult(MediaType type, object content)
        {
            Type = type;
            Content = content;
        }
    }
}