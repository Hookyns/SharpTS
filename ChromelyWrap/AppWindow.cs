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
		/// Called after application load finish including UI(/client) part
		/// </summary>
		public event Action Loaded;

		#endregion

		#region Fields

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
			this.FrameLoadEnd += this.HandleFrameLoadEnd;
			this.Loaded += this.OnLoaded;
		}

		#endregion

		#region Methods
		

		#endregion

		#region Private methods

		/// <summary>
		/// Frame load end event handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
		{
			this.Loaded?.Invoke();
		}

		/// <summary>
		/// Called when window launched
		/// </summary>
		private void OnLoaded()
		{
			// Proccess debug mode
			if (this.debugMode)
			{
				this.ShowDevTools();
			}
		}

		/// <summary>
		/// Show DevTools window
		/// </summary>
		public void ShowDevTools()
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