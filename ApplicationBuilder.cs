using System;
using System.Linq;
using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Infrastructure;
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
		private static ApplicationBuilder instance;

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

		// /// <summary>
		// /// Window instance
		// /// </summary>
		// public AppWindow AppWindow { get; private set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="application">Application name</param>
		private ApplicationBuilder(Application application)
		{
			this.Initialize(application);
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
		public static ApplicationBuilder UseApp<TApp>(TApp application = null)
		where TApp : Application, new()
		{
			if (application == null)
			{
				application = new TApp();
			}

			lock (@lock)
			{
				if (instance != null)
				{
					throw new InvalidOperationException("You can build only one application!");
				}
				
				instance = new ApplicationBuilder(application);
			}

			return instance;
		}

		/// <summary>
		/// Start application
		/// </summary>
		public void Start(string startUrl = null)
		{
			// this.AppWindow.Configure(config =>
			// {
			// 	config
			// 		.WithDefaultSubprocess()
			// 		.WithHostTitle(this.application.Name)
			// 		.WithSilentCefBinariesLoading(true)
			// 		.UseDefaultResourceSchemeHandler("local", string.Empty)
			// 		.WithStartUrl(string.IsNullOrWhiteSpace(startUrl) ? StartUrl : startUrl)
			// 		.RegisterMessageRouterHandler(this.messageBroker.MessageHandler)
			// 		;
			// });
			//
			// return this.AppWindow.Create();
			
			string[] args = Environment.GetCommandLineArgs()
				// Skip first, it's program path 
				.Skip(1)
				.ToArray();
			
			config.StartUrl = string.IsNullOrWhiteSpace(startUrl) ? StartUrl : startUrl;
			
			// Blocking call
			this.appBuilder
				.Build()
				.Run(args);
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

		#endregion

		#region Private methods

		/// <summary>
		/// Initialize application
		/// </summary>
		/// <param name="application"></param>
		private void Initialize(Application application)
		{
			this.PrepareConfig();
			
			// TODO: Create custom application, handling frameless and using specific chromely window
			this.sharpTsApp = new SharpTsBasicApplication(application);

			this.appBuilder = AppBuilder.Create()
				.UseConfig<DefaultConfiguration>(this.config)
				.UseWindow<AppWindow>()
				.UseApp<ChromelyBasicApp>(this.sharpTsApp);
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
			this.config.CustomSettings[CefSettingKeys.LOCALESDIRPATH] = ".\\AppData";
			this.config.CustomSettings[CefSettingKeys.CACHEPATH] = ".\\AppData\\Cache";
			this.config.CustomSettings[CefSettingKeys.USERDATAPATH] = ".\\UserData";
			
			config.CefDownloadOptions = new CefDownloadOptions(true, false); // TODO: true, true
			
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
	}
}