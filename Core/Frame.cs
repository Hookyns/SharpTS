using System;
using SharpTS.Message;
using SharpTS.Message.Messages;

namespace SharpTS.Core
{
	/// <summary>
	/// Navigatable frame
	/// </summary>
	public sealed class Frame
	{
		/// <summary>
		/// Frame name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Message broker
		/// </summary>
		private readonly MessageBroker messageBroker;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="messageBroker"></param>
		internal Frame(string name, MessageBroker messageBroker)
		{
			this.Name = name;
			this.messageBroker = messageBroker;
		}

		/// <summary>
		/// Navigate to given page
		/// </summary>
		/// <param name="pageType"></param>
		public void Navigate(Type pageType)
		{
			this.messageBroker.Send(new DataMessage<FrameNavigateMessageData>(MessageType.Navigate)
			{
				Data = new FrameNavigateMessageData()
				{
					PageName = pageType.AssemblyQualifiedName,
					FrameName = this.Name
				}
			});
		}
	}
}