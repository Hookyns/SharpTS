using Newtonsoft.Json.Linq;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace SharpTS.Message
{
	/// <summary>
	/// Message argument object
	/// </summary>
	public class MessageEventArgs
	{
		#region Fields

		/// <summary>
		/// Callback
		/// </summary>
		private CefMessageRouterBrowserSide.Callback callback;

		/// <summary>
		/// Marked true when served by somebody.
		/// </summary>
		private bool served;

		#endregion
		
		#region Properties

		/// <summary>
		/// Base message
		/// </summary>
		internal CefFrame CefFrame { get; }

		/// <summary>
		/// Base message
		/// </summary>
		public BaseMessage BaseMessage { get; }

		/// <summary>
		/// Raw JSON message
		/// </summary>
		public string RawJson { get; }

		/// <summary>
		/// Parsed JSON
		/// </summary>
		public JObject ParsedJson { get; }

		/// <summary>
		/// PostData of parsed JSON
		/// </summary>
		public JToken PostData { get; }

		/// <summary>
		/// True if served by some code/service/somebodyw
		/// </summary>
		public bool Served => this.served;

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="cefFrame"></param>
		/// <param name="rawJson"></param>
		/// <param name="callback"></param>
		public MessageEventArgs(CefFrame cefFrame, string rawJson, CefMessageRouterBrowserSide.Callback callback)
		{
			this.CefFrame = cefFrame;
			this.callback = callback;
			
			this.RawJson = rawJson;
			this.ParsedJson = JObject.Parse(rawJson);
			this.PostData = this.ParsedJson.GetValue("postData");

			this.BaseMessage = this.PostData.ToObject<BaseMessage>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Return required data from message
		/// </summary>
		/// <typeparam name="TData"></typeparam>
		/// <returns></returns>
		public TData GetData<TData>()
		{
			return this.PostData.ToObject<TData>();
		}

		/// <summary>
		/// Send success response
		/// </summary>
		/// <param name="response"></param>
		public void Success(string response = null)
		{
			if (this.served)
			{
				return;
			}
			this.served = true;
			
			this.callback.Success(response);
		}

		/// <summary>
		/// Send failure response
		/// </summary>
		/// <param name="errorCode"></param>
		/// <param name="errorMessage"></param>
		public void Failure(int errorCode, string errorMessage)
		{
			if (this.served)
			{
				return;
			}
			this.served = true;
			
			this.callback.Failure(errorCode, errorMessage);
		}

		#endregion
	}
}