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
        /// Factory which should be used
        /// </summary>
        public static ILoggerFactory Factory;
        
        /// <summary>
        /// Create logger
        /// </summary>
        /// <typeparam name="TCategory"></typeparam>
        /// <returns></returns>
        public static ILogger<TCategory> CreateLogger<TCategory>()
        {
            if (Factory == null)
            {
                return NullLogger<TCategory>.Instance;
            }
            
            return Factory.CreateLogger<TCategory>();
        }
        
        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static ILogger CreateLogger(string categoryName)
        {
            if (Factory == null)
            {
                return NullLogger<NullLogger>.Instance;
            }
            
            return Factory.CreateLogger(categoryName);
        }
    }
}