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

        /// <summary>
        /// Open DevTools on application strt
        /// </summary>
        internal bool StartWithDevToolsOpened { get; private set; }

        /// <summary>
        /// Application instance
        /// </summary>
        internal Application Application { get; private set; }

        /// <summary>
        /// Start application without window frame
        /// </summary>
        internal bool StartFrameless { get; set; }

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

            Logging.LoggerFactory.Factory = loggerFactory;

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

        /// <summary>
        /// Start application withDevTools opened
        /// </summary>
        /// <returns></returns>
        public ApplicationBuilder StartWithDevTools()
        {
            this.StartWithDevToolsOpened = true;
            return this;
        }

        public ApplicationBuilder Frameless()
        {
            this.StartFrameless = true;
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

            this.config.StartUrl = string.IsNullOrWhiteSpace(startUrl) ? StartUrl : startUrl;
            
            this.sharpTsApp = this.StartFrameless ? (ChromelyAppBase)new SharpTsFramelessApplication(this) : new SharpTsBasicApplication(this);

            AppBuilder.Create()
                .UseConfig<DefaultConfiguration>(this.config)
                .UseWindow<AppWindow>()
                .UseApp<ChromelyBasicApp>(this.sharpTsApp)
                .Build()
                // Blocking call
                .Run(args);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initialize application
        /// </summary>
        private void Initialize()
        {
            this.PrepareConfig();
        }

        /// <summary>
        /// Prepare new config object
        /// </summary>
        /// <returns></returns>
        private void PrepareConfig()
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

            this.config.CefDownloadOptions = new CefDownloadOptions(true, false); // TODO: true, true

            this.config.WindowOptions.Title = this.Application.Name;
            this.config.WindowOptions.StartCentered = true;
            // config.WindowOptions.Position = new WindowPosition(1, 2);
            this.config.WindowOptions.Size = new WindowSize(1000, 600);
            this.config.WindowOptions.RelativePathToIconFile = this.Application.Icon;
            
            this.config.DebuggingMode = false;
        }

        #endregion
    }
}