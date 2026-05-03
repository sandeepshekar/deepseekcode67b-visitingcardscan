// GalleryView.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PhotoViewerApp.Models;
using PhotoViewerApp.ViewModels;

namespace PhotoViewerApp.Controls
{
    /// <summary>
    /// Interaction logic for GalleryView.
    /// </summary>
    public sealed partial class GalleryView : UserControl
    {
        public GalleryView()
        {
            this.InitializeComponent();
            // Attach the event handler manually since XAML binding is complex here,
            // but we'll keep it conceptually simple for now.
        }

        private async void Thumbnail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is MediaFile mediaFile)
            {
                // When clicked, fire the event/command that triggers the ViewModel to update the selection
                if (this.DataContext is GalleryViewModel viewModel)
                {
                    await viewModel.HandleSelectionChanged(mediaFile);
                }
            }
        }
    }