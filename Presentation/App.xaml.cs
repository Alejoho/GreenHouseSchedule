using log4net;
using SupportLayer;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log = LogHelper.GetLogger();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _log.Info("The app was launched");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _log.Info("The app was exited");
        }

        //NEXT - Ver cual es el evento que se dispara al abrir y cerrar la app y poner un log en los dos.
    }
}
