using Logic;
using Presentation.Model;
using Presentation.ViewModel;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(new MDDialogHost(), new FileRepository());
        }
    }
}
