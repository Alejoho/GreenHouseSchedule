using log4net;
using SupportLayer;
using System;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log = LogHelper.GetLogger();
        private bool _wasTheMessageDisplayed;

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _wasTheMessageDisplayed = false;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _log.Info("The app was launched");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _log.Info("The app was exited");
            _log.Info("____________________________________________________________" +
                "____________________________________________________________________" +
                "________________________");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            var ex = e.Exception;
            _log.Fatal("It was an unexpected error catch by the DispatcherUnhandledException and the app crashed", ex);
            _log.Fatal("____________________________________________________________" +
                "____________________________________________________________________" +
                "________________________");

            if (_wasTheMessageDisplayed == false)
            {
                MessageBox.Show("Hubo un error inesperado. La applicación va a cerrarse."
                    , "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);

                _wasTheMessageDisplayed = true;
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;

            _log.Fatal("It was an unexpected error catch by the UnhandledException and the app crashed", ex);
            _log.Fatal("____________________________________________________________" +
                "____________________________________________________________________" +
                "________________________");

            if (_wasTheMessageDisplayed == false)
            {
                MessageBox.Show("Hubo un error inesperado. La applicación va a cerrarse."
                    , "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);

                _wasTheMessageDisplayed = true;
            }
        }
    }
}
