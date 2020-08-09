using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SharpTS.Logging
{
    /// <summary>
    /// Logging factory
    /// </summary>
    internal static class LoggerFactory
    {
        /// <summary>
        /// Factoy which should be used
        /// </summary>
        public static ILoggerFactory Factory;
        
        /// <summary>
        /// Create logger
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> CreateLogger<T>()
        {
            if (Factory == null)
            {
                return NullLogger<T>.Instance;
            }
            
            return Factory.CreateLogger<T>();
        }
    }
}