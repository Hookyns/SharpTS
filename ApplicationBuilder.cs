using System;
using System.IO;
using System.Linq;
using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using SharpTS.ChromelyWrap;
using SharpTS.Core;

namespace SharpTS
{
    /// <summary>
    /// Application framework class
    /// </summary>
    public class ApplicationBuilder
    {
        #region Fields

        /// <summary>
        /// Start-up file
        /// </summary>
        const string StartUrl = "local://UI/dist/index.html";

        /// <summary>
        /// Static locking object
        /// </summary>
        private static object @lock = new object();

        /// <summary>
        /// Application singleton instance
        /// </summary>
        private static ApplicationBuilder _instance;

        // /// <summary>
        // /// Application instance
        // /// </summary>
        // private Application application;

        // /// <summary>
        // /// DI service collection
        // /// </summary>
        // private readonly ServiceCollection serviceCollection = new ServiceCollection();
        //
        // /// <summary>
        // /// DI service provider
        // /// </summary>
        // private ServiceProvider serviceProvider;

        /// <summary>
        /// Chromely AppBuilder
        /// </summary>
        private AppBuilder appBuilder;

        /// <summary>
        /// Chromely app config
        /// </summary>
        private IChromelyConfiguration config;

        /// <summary>
        /// Instance of SharpTs application
        /// </summary>
        private ChromelyAppBase sharpTsApp;

        #endregion

        #region Properties

        public ILoggerFactory LoggerFactory { get; private set; }

        public bool StartWithDevToolsOpened { get; private set; }

        #endregion

        #region Ctors

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="application">Application name</param>
        private ApplicationBuilder(Application application)
        {
            this.Application = application;
            
            this.Initialize();
        }

        /// <summary>
        /// Application instance
        /// </summary>
        public Application Application { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create builder over given application
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static ApplicationBuilder ForApp<TApp>(TApp application = null)
            where TApp : Application, new()
        {
            if (application == null)
            {
                application = new TApp();
            }

            lock (@lock)
            {
                if (_instance != null)
                {
                    throw new InvalidOperationException("You can build only one application!");
                }

                _instance = new ApplicationBuilder(application);
            }

            return _instance;
        }

        /// <summary>
        /// Enable debug mode
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public ApplicationBuilder UseLogger<TLoggerFactory>(TLoggerFactory loggerFactory)
            where TLoggerFactory : ILoggerFactory
        {
            if (loggerFactory == null)
            {
                loggerFactory = Activator.CreateInstance<TLoggerFactory>();
            }

            this.LoggerFactory = loggerFactory;

            return this;
        }

        /// <summary>
        /// Enable debug mode
        /// </summary>
        /// <param name="debugPort"></param>
        /// <returns></returns>
        public ApplicationBuilder Debug(int debugPort = 20480)
        {
            this.config.DebuggingMode = true;
            this.config.CustomSettings[CefSettingKeys.NOSANDBOX] = "false";
            this.config.CustomSettings[CefSettingKeys.REMOTEDEBUGGINGPORT] = debugPort.ToString();
            this.config.CustomSettings.Remove(CefSettingKeys.LOCALESDIRPATH);
            this.config.CustomSettings.Remove(CefSettingKeys.CACHEPATH);
            this.config.CustomSettings.Remove(CefSettingKeys.USERDATAPATH);

            return this;
        }

        public ApplicationBuilder StartWithDevTools()
        {
            this.StartWithDevToolsOpened = true;
            return this;
        }

        /// <summary>
        /// Start application
        /// </summary>
        public void Start(string startUrl = null)
        {
            string[] args = Environment.GetCommandLineArgs()
                // Skip first, it's program path 
                .Skip(1)
                .ToArray();

            config.StartUrl = string.IsNullOrWhiteSpace(startUrl) ? StartUrl : startUrl;

            // TOOD: zavolat metodu Setup(), aby sez ApplicationBuilderu nastavilo všechno potřebné
            
            // Blocking call
            this.appBuilder
                .Build()
                .Run(args);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initialize application
        /// </summary>
        private void Initialize()
        {
            this.PrepareConfig(this.Application);

            // TODO: Create custom application, handling frameless and using specific chromely window
            this.sharpTsApp = new SharpTsBasicApplication(this);

            this.appBuilder = AppBuilder.Create()
                .UseConfig<DefaultConfiguration>(this.config)
                .UseWindow<AppWindow>()
                .UseApp<ChromelyBasicApp>(this.sharpTsApp);
        }

        /// <summary>
        /// Prepare new config object
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        private void PrepareConfig(Application application)
        {
            // string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            this.config = DefaultConfiguration.CreateForRuntimePlatform();

            // config.B
            // .WithAppArgs(args)
            // .WithHostBounds(1000, 600)

            // .WithShutdownCefOnExit(true)
            // .WithHostMode(WindowState.Normal)
            // .WithHostFlag(HostFlagKey.Frameless, true) / TODO: Frameless option

            // .WithHostCustomStyle(windowStyle)
            // .WithCustomSetting(CefSettingKeys.NoSandbox, "true")
//					.WithCustomSetting(CefSettingKeys.ResourcesDirPath, ".\\AppData")

            this.config.CustomSettings[CefSettingKeys.NOSANDBOX] = "true";
            // TODO: Move to AppData; need to move it after Cef binaries download
            // this.config.CustomSettings[CefSettingKeys.LOCALESDIRPATH] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "Locales");
            this.config.CustomSettings[CefSettingKeys.CACHEPATH] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "Cache");
            this.config.CustomSettings[CefSettingKeys.USERDATAPATH] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserData");

            config.CefDownloadOptions = new CefDownloadOptions(true, false); // TODO: true, true

            config.WindowOptions.Title = application.Name;
            this.config.WindowOptions.StartCentered = true;
            // config.WindowOptions.Position = new WindowPosition(1, 2);
            config.WindowOptions.Size = new WindowSize(1000, 600);
            config.DebuggingMode = false;

            config.WindowOptions.RelativePathToIconFile = "chromely.ico"; // TODO: Make it configurable

            // 		.WithDefaultSubprocess()
            // 		.WithHostTitle(this.application.Name)
            // 		.WithSilentCefBinariesLoading(true)
            // 		.UseDefaultResourceSchemeHandler("local", string.Empty)
            // 		.WithStartUrl(string.IsNullOrWhiteSpace(startUrl) ? StartUrl : startUrl)
            // 		.RegisterMessageRouterHandler(this.messageBroker.MessageHandler)
        }

        #endregion

        // /// <summary>
        // /// Window instance
        // /// </summary>
        // public AppWindow AppWindow { get; private set; }
    }
}