using System;
using System.Windows;

using Logic.Client;

using Presentation.Model;
using Presentation.ViewModel;

namespace Presentation.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Closing += MainWindow_Closing;
            DataContext = new MainViewModel(new MDDialogHost(), new WebRepository(), "0", "1");
            InitializeComponent();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainViewModel)DataContext as IDisposable)?.Dispose();
        }
    }
}
