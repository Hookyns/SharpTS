using Chromely;
using Microsoft.Extensions.DependencyInjection;

namespace SharpTS.ChromelyWrap
{
    public sealed class SharpTsFramelessApplication : ChromelyFramelessApp
    {
        #region Fields

        /// <summary>
        /// Composition, base SharpTS application instance
        /// </summary>
        private readonly SharpTsApplication sharpTsApplication;

        #endregion

        #region Ctors

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public SharpTsFramelessApplication(ApplicationBuilder applicationBuilder)
        {
            this.sharpTsApplication = new SharpTsApplication(applicationBuilder);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configure DI services
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void ConfigureServices(ServiceCollection serviceCollection)
        {
            base.ConfigureServices(serviceCollection);
            this.sharpTsApplication.RegisterInternalServices(serviceCollection);
        }

        /// <summary>
        /// Initialize application after setup
        /// </summary>
        /// <param name="serviceProvider"></param>
        public override void Initialize(ServiceProvider serviceProvider)
        {
            base.Initialize(serviceProvider);

            this.sharpTsApplication.PrepareMessageBroker(serviceProvider);
            this.sharpTsApplication.ConfigureApplication(serviceProvider);
            this.sharpTsApplication.Configure(serviceProvider);
            this.sharpTsApplication.InitializeApplication();
        }

        #endregion
    }
}