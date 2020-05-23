using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace SharpTS.Message
{
	/// <summary>
	/// Ibterop message handler
	/// </summary>
	public class InteropMessageHandler : CefMessageRouterBrowserSide.Handler
	{
		#region Events

		/// <summary>
		/// Message event handler delegate
		/// </summary>
		public delegate void OnMessageHandler(MessageEventArgs args);

		/// <summary>
		/// Message event
		/// </summary>
		public event OnMessageHandler OnMessage;

		#endregion

		#region Fields

		/// <summary>
		/// Dictionary with queries's cancel tokens
		/// </summary>
		private ConcurrentDictionary<long, CancellationTokenSource> QueryCancelTokens { get; } = new ConcurrentDictionary<long, CancellationTokenSource>();

		#endregion
		
		#region Methods

		/// <summary>
		/// On query income
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="frame"></param>
		/// <param name="queryId"></param>
		/// <param name="request"></param>
		/// <param name="persistent"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public override bool OnQuery(CefBrowser browser, CefFrame frame, long queryId, string request, bool persistent,
			CefMessageRouterBrowserSide.Callback callback)
		{
			try
			{
				var cts = new CancellationTokenSource();
				this.QueryCancelTokens.TryAdd(queryId, cts);

				Task.Run(() =>
				{
					try
					{
						this.OnMessage?.Invoke(new MessageEventArgs(request, callback));
					}
					catch (Exception ex)
					{
						callback.Failure(-1, ex.Message);
					}

					this.DisposeCancelTokenSource(queryId);
				}, cts.Token);

				return true;
			}
			catch (Exception ex)
			{
				Debug.Fail(ex.StackTrace);
				callback.Failure(-1, ex.Message);
			}

			this.DisposeCancelTokenSource(queryId);
			
			return false;
		}

		/// <summary>
		/// On query cancel
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="frame"></param>
		/// <param name="queryId"></param>
		public override void OnQueryCanceled(CefBrowser browser, CefFrame frame, long queryId)
		{
			CancellationTokenSource cts;

			if (this.QueryCancelTokens.TryRemove(queryId, out cts))
			{
				cts.Cancel();
				cts.Dispose();
			}
			else
			{
				// Too early or too late?!
				Debug.WriteLine("Query cancel requested but no cancel token found.");
			}
		}
		
		#endregion
		
		#region Private methods

		/// <summary>
		/// Dispose CancellationTokenSource of given query
		/// </summary>
		/// <param name="queryId"></param>
		private void DisposeCancelTokenSource(long queryId)
		{
			CancellationTokenSource cts;
			
			if (this.QueryCancelTokens.TryRemove(queryId, out cts))
			{
				cts.Dispose();
			}
		}
		
		#endregion
	}
}