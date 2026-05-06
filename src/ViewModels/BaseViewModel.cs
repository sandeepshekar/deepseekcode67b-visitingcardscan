// BaseViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoViewerApp.ViewModels
{
    /// <summary>
    /// Provides basic INotifyPropertyChanged implementation for all ViewModels.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}