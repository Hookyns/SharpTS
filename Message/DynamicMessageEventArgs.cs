using Newtonsoft.Json.Linq;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace SharpTS.Message
{
	/// <summary>
	/// Message argument object
	/// </summary>
	public class DynamicMessageEventArgs : MessageEventArgs
	{
		#region Properties

		/// <summary>
		/// Parsed JSON
		/// </summary>
		internal JObject ParsedJson { get; }

		/// <summary>
		/// PostData of parsed JSON
		/// </summary>
		public JToken MessageData { get; }

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="cefFrame"></param>
		/// <param name="rawJson"></param>
		/// <param name="callback"></param>
		public DynamicMessageEventArgs(CefFrame cefFrame, string rawJson, CefMessageRouterBrowserSide.Callback callback)
		: base(cefFrame, rawJson, callback)
		{
			this.ParsedJson = JObject.Parse(rawJson);
			this.MessageData = this.ParsedJson.GetValue("postData");

			this.BaseMessage = this.MessageData.ToObject<BaseMessage>();
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
			return this.MessageData.ToObject<TData>();
		}
		
		#endregion
	}
}