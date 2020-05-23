using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chromely.CefGlue;
using Chromely.CefGlue.Winapi;
using Chromely.Core;
using Chromely.Core.Host;
using WinApi.User32;
using Xilium.CefGlue;

namespace SharpTS.Chromely
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

			// Start Task watching window status and running Init
			this.WaitForInit(window);

			// Blocking call
			return this.window.Run(this.config.AppArgs);
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
//					.WithDebuggingMode(true)
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

		#endregion
	}
}