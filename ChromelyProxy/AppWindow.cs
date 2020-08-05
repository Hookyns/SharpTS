using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chromely.CefGlue;
using Chromely.CefGlue.Browser;
using Chromely.CefGlue.Winapi;
using Chromely.Core;
using Chromely.Core.Helpers;
using Chromely.Core.Host;
using WinApi.User32;
using Xilium.CefGlue;

namespace SharpTS.ChromelyProxy
{
	/// <summary>
	/// Application (browser) window
	/// </summary>
	public class AppWindow : IDisposable
	{
		#region Events

		public event Action OnLaunched;

		#endregion

		#region Fields

		/// <summary>
		/// Chromely configuration
		/// </summary>
		private ChromelyConfiguration config;

		/// <summary>
		/// Chromely window
		/// </summary>
		private IChromelyWindow window;

		/// <summary>
		/// Debug mode enabled
		/// </summary>
		private bool debugMode;

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		public AppWindow()
		{
			this.Initialize();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Change default configuration of Chromely window
		/// </summary>
		/// <param name="configurator"></param>
		public void Configure(Action<ChromelyConfiguration> configurator)
		{
			configurator.Invoke(this.config);
		}

		/// <summary>
		/// Create and show app (browser) window
		/// </summary>
		/// <returns></returns>
		public int Create()
		{
			this.window = ChromelyWindow.Create(this.config);

			// Register own launch event handler
			this.OnLaunched += OnOnLaunched;

			// Start Task watching window status and running Init
			this.WaitForInit(window);

			// Blocking call
			return this.window.Run(this.config.AppArgs);
		}

		/// <summary>
		/// Enable debug mode
		/// </summary>
		public void Debug(int debugPort = 20480)
		{
			this.debugMode = true;

			this.config = this.config
				.WithDebuggingMode(true)
				.WithCustomSetting(CefSettingKeys.NoSandbox, "false")
//					.WithCustomSetting(CefSettingKeys.ResourcesDirPath, null)
				.WithCustomSetting(CefSettingKeys.LocalesDirPath, null)
				.WithCustomSetting(CefSettingKeys.RemoteDebuggingPort, debugPort)
				.WithCustomSetting(CefSettingKeys.CachePath, null)
				.WithCustomSetting(CefSettingKeys.UserDataPath, null);
		}

		/// <summary>
		/// Dispose window
		/// </summary>
		public void Dispose()
		{
			this.window?.Dispose();
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Initialization method
		/// </summary>
		private void Initialize()
		{
			HostHelpers.SetupDefaultExceptionHandlers();
			this.config = PrepareConfig();
		}

		/// <summary>
		/// Prepare new config object
		/// </summary>
		/// <returns></returns>
		private static ChromelyConfiguration PrepareConfig()
		{
			var windowStyle = new WindowCreationStyle()
			{
				WindowStyles = WindowStyles.WS_CAPTION | WindowStyles.WS_CLIPCHILDREN |
					WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_GROUP | WindowStyles.WS_MAXIMIZEBOX |
					WindowStyles.WS_POPUP | WindowStyles.WS_SIZEBOX,
				WindowExStyles = WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_WINDOWEDGE |
					WindowExStyles.WS_EX_TRANSPARENT,
			};

			string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

			var config = ChromelyConfiguration
				.Create()
				.WithAppArgs(args)
				.WithHostBounds(1000, 600)
				.WithShutdownCefOnExit(true)
				.WithHostMode(WindowState.Normal)
				.WithHostFlag(HostFlagKey.Frameless, true)
				.WithHostCustomStyle(windowStyle)
				.WithCustomSetting(CefSettingKeys.NoSandbox, "true")
//					.WithCustomSetting(CefSettingKeys.ResourcesDirPath, ".\\AppData")
				.WithCustomSetting(CefSettingKeys.LocalesDirPath, ".\\AppData")
				.WithCustomSetting(CefSettingKeys.CachePath, ".\\AppData\\Cache")
				.WithCustomSetting(CefSettingKeys.UserDataPath, ".\\UserData")
				.WithDebuggingMode(false);
			;

			return config;
		}

		/// <summary>
		/// Wait for init
		/// </summary>
		/// <param name="window"></param>
		/// <returns></returns>
		private Task WaitForInit(IChromelyWindow window)
		{
			return Task.Run(() =>
			{
				AutoResetEvent are = new AutoResetEvent(false);

				while (!are.WaitOne(TimeSpan.FromMilliseconds(10)))
				{
					if (window.Browser != null)
					{
						are.Set();
					}
				}

				this.OnLaunched?.Invoke();
			});
		}

		/// <summary>
		/// Called when window launched
		/// </summary>
		private void OnOnLaunched()
		{
			// Proccess debug mode
			if (this.debugMode)
			{
				this.ShowDevTools();
			}
		}

		private void ShowDevTools()
		{
			CefTaskRunner.GetForThread(CefThreadId.UI).PostTask(new GenericCefTask(() =>
			{
				var cefGlueBrowser = this.window.Browser as CefGlueBrowser;
			
				if (cefGlueBrowser == null)
				{
					throw new InvalidOperationException("CefGlue browser is not used.");
				}
				
				CefBrowser browser = cefGlueBrowser.CefBrowser;
				CefBrowserHost host = browser.GetHost();
				CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
				cefWindowInfo.SetAsPopup(IntPtr.Zero, "DevTools");
				CefWindowInfo windowInfo = cefWindowInfo;
				DevToolsWebClient devToolsWebClient = new DevToolsWebClient();
				CefBrowserSettings browserSettings = new CefBrowserSettings();
				CefPoint inspectElementAt = new CefPoint(0, 0);
				host.ShowDevTools(windowInfo, devToolsWebClient, browserSettings, inspectElementAt);
			}));
		}

		#endregion

		#region Classes

		private class DevToolsWebClient : CefClient
		{
		}

		#endregion
	}
}