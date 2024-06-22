using log4net;
using System.Runtime.CompilerServices;

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
