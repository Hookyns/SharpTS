using System;
using SharpTS.Message;
using SharpTS.Message.Messages;
using Xilium.CefGlue;

namespace SharpTS.Core
{
	/// <summary>
	/// Navigatable frame
	/// </summary>
	public sealed class Frame
	{
		/// <summary>
		/// Owning CefFrame
		/// </summary>
		private readonly CefFrame cefFrame;

		/// <summary>
		/// Message broker
		/// </summary>
		private readonly MessageBroker messageBroker;
		
		/// <summary>
		/// Frame name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="cefFrame"></param>
		/// <param name="messageBroker"></param>
		internal Frame(string name, CefFrame cefFrame, MessageBroker messageBroker)
		{
			this.Name = name;
			this.cefFrame = cefFrame;
			this.messageBroker = messageBroker;
		}

		/// <summary>
		/// Navigate to given page
		/// </summary>
		/// <param name="pageType"></param>
		public void Navigate(Type pageType)
		{
			this.messageBroker.Send(this.cefFrame, new DataMessage<FrameNavigateMessageData>(MessageType.Navigate)
			{
				Data = new FrameNavigateMessageData()
				{
					PageName = pageType.FullName,
					FrameName = this.Name
				}
			});
		}
	}
}