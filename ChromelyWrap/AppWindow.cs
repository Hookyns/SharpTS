using System;
using Chromely;
using Chromely.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Host;
using Xilium.CefGlue;

namespace SharpTS.ChromelyWrap
{
	/// <summary>
	/// Application (browser) window
	/// </summary>
	public class AppWindow : Window
	{
		#region Events

		/// <summary>
		/// Call right after Browser instance creation
		/// </summary>
		public event Action BrowserCreated;
		
		/// <summary>
		/// Called after application load finish including UI(/client) part
		/// </summary>
		public event Action OnLaunched;

		#endregion

		#region Fields

		// /// <summary>
		// /// Chromely configuration
		// /// </summary>
		// private IChromelyConfiguration config;

		// /// <summary>
		// /// Chromely window
		// /// </summary>
		// private IChromelyWindow window;

		/// <summary>
		/// Debug mode enabled
		/// </summary>
		private bool debugMode; // TODO: allow to enable based on AppBuilder setting

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		public AppWindow(IChromelyNativeHost nativeHost,
			IChromelyConfiguration config,
			ChromelyHandlersResolver handlersResolver
		)
			: base(nativeHost, config, handlersResolver)
		{
			FrameLoadEnd += HandleFrameLoadEnd;
			this.OnLaunched += OnOnLaunched;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Method creating browser
		/// </summary>
		/// <param name="hostHandle"></param>
		/// <param name="winXID"></param>
		public override void Create(IntPtr hostHandle, IntPtr winXID)
		{
			base.Create(hostHandle, winXID);
			
			// Call registered handlers
			this.BrowserCreated?.Invoke();
		}

		// /// <summary>
		// /// Change default configuration of Chromely window
		// /// </summary>
		// /// <param name="configurator"></param>
		// public void Configure(Action<IChromelyConfiguration> configurator)
		// {
		// 	configurator.Invoke(this.config);
		// }

		// /// <summary>
		// /// Create and show app (browser) window
		// /// </summary>
		// /// <returns></returns>
		// public int Create()
		// {
		// 	// this.window = ChromelyWindow.Create(this.config);
		//
		// 	// Register own launch event handler
		// 	this.OnLaunched += OnOnLaunched;
		//
		// 	// // Start Task watching window status and running Init
		// 	// this.WaitForInit(window);
		//
		// 	// // Blocking call
		// 	// return this.window.Run(this.config.AppArgs);
		// }


		#endregion

		#region Private methods

		// /// <summary>
		// /// Initialization method
		// /// </summary>
		// private void Initialize()
		// {
		// 	HostHelpers.SetupDefaultExceptionHandlers();
		// 	this.config = PrepareConfig();
		// }

		/// <summary>
		/// Frame load end event handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
		{
			this.OnLaunched?.Invoke();
		}



		// /// <summary>
		// /// Wait for init
		// /// </summary>
		// /// <param name="window"></param>
		// /// <returns></returns>
		// private Task WaitForInit(IChromelyWindow window)
		// {
		// 	return Task.Run(() =>
		// 	{
		// 		AutoResetEvent are = new AutoResetEvent(false);
		//
		// 		while (!are.WaitOne(TimeSpan.FromMilliseconds(10)))
		// 		{
		// 			if (window.Browser != null)
		// 			{
		// 				are.Set();
		// 			}
		// 		}
		//
		// 		this.OnLaunched?.Invoke();
		// 	});
		// }

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
			CefBrowser browser = this.Browser;
			CefBrowserHost host = browser.GetHost();
			CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
			cefWindowInfo.SetAsPopup(IntPtr.Zero, "DevTools");
			CefWindowInfo windowInfo = cefWindowInfo;
			DevToolsWebClient devToolsWebClient = new DevToolsWebClient();
			CefBrowserSettings browserSettings = new CefBrowserSettings();
			CefPoint inspectElementAt = new CefPoint(0, 0);
			host.ShowDevTools(windowInfo, devToolsWebClient, browserSettings, inspectElementAt);
		}

		#endregion

		#region Classes

		private class DevToolsWebClient : CefClient
		{
		}

		#endregion
	}
}