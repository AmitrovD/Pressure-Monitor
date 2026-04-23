using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressureMonitor.Helpers
{
    public static class AppLoggerFactory
    {
        private static ILoggerFactory? _factory;
        public static void Initialize()
        {
            _factory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddDebug(); 
            });
            var testLogger = _factory.CreateLogger("Test");
            testLogger.LogInformation("Логгер инициализирован!");
            System.Windows.MessageBox.Show("Логгер создан");
        }
        public static ILogger<T> CreateLogger<T>()
        {
            if (_factory == null)
                Initialize();

            return _factory!.CreateLogger<T>();
        }
    }
}
