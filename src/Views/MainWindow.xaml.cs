using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PhotoViewerApp.ViewModels;
using PhotoViewerApp.Services.Interfaces;
using PhotoViewerApp.Services;
using PhotoViewerApp.Models;

namespace PhotoViewerApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly GalleryViewModel _viewModel;
        // We need services accessible to the main window scope, simulating DI
        private readonly IFileScanService _fileScanner;
        private readonly IThumbnailService _thumbnailService;
        private readonly IMediaService _mediaService;

        public MainWindow()
        {
            this.InitializeComponent();

            // --- 1. Dependency Resolution (Simulating IoC/DI Container) ---
            _fileScanner = new FileScanService();
            _thumbnailService = new ThumbnailService();
            _mediaService = new MediaService(_thumbnailService);

            // Initialize ViewModel with all dependencies
            _viewModel = new GalleryViewModel(_fileScanner, _thumbnailService, _mediaService);
            this.DataContext = _viewModel;

            // 2. Setup the main action: Load initial folder
            InitializeFolderPicker();
        }

        private void InitializeFolderPicker()
        {
            // Assuming there's a button in MainWindow.xaml called 'ScanButton'
            // We attach an event handler to it.
            if (this.FindName("ScanButton") is Button scanButton)
            {
                scanButton.Click += async (sender, e) => await ScanAndLoadDirectory();
            }
        }

        private async Task ScanAndLoadDirectory()
        {
            // In a real app, we'd use a FilePicker dialog here. For simulation, we'll ask the user or assume a path.
            string selectedPath = "C:\\Users\\Public\\Pictures"; // Placeholder Path! User must change this for testing
            if (!Directory.Exists(selectedPath))
            {
                 MessageBox.Show("Please set a valid local directory path to test.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kick off the entire pipeline: Scan -> Thumbnail Generation -> View Model Update
            await _viewModel.LoadGalleryAsync(selectedPath);
        }
    }
}