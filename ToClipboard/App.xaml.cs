using System.Windows;

namespace ToClipboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool HideOnRun { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!HideOnRun)
                new MainWindow().Show();
            else
            {
                ToClipboard.MainWindow.CreateJumpList();
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
