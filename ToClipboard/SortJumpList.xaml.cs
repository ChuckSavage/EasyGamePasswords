using System.Windows;

namespace ToClipboard
{
    /// <summary>
    /// Interaction logic for SortJumpList.xaml
    /// </summary>
    public partial class SortJumpList : Window
    {
        public SortJumpList()
        {
            InitializeComponent();
        }

        private void CreateJumpList(object sender, RoutedEventArgs e)
        {
            MainWindow.CreateJumpList();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
