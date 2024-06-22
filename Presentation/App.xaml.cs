using System.Windows;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //NEXT - Ver cual es el evento que se dispara al abrir y cerrar la app y poner un log en los dos.
    }
}
