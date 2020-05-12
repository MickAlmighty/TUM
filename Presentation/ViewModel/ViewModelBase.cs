using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Presentation.ViewModel
{
    /// <summary>
    ///     Acts as a base for ViewModels.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises PropertyChanged event with appropriate property name. Used to update bindings.
        /// </summary>
        /// <param name="propertyName">
        ///     Exact name of the property which was modified.
        ///     If this argument is omitted, it's replaced with the name of the calling property/method.
        /// </param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
