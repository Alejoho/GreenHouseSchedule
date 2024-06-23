using log4net;
using System.Runtime.CompilerServices;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SupportLayer
{
    public static class LogHelper
    {
        public static ILog GetLogger([CallerFilePath] string filePath = "")
        {
            return LogManager.GetLogger(filePath);
        }
    }
}
