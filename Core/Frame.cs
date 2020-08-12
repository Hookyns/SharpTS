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
		#region Fields

		/// <summary>
		/// Owning CefFrame
		/// </summary>
		private readonly CefFrame cefFrame;

		/// <summary>
		/// Message broker
		/// </summary>
		private readonly MessageBroker messageBroker;

		#endregion

		#region Properties

		/// <summary>
		/// Frame name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Hold last component type
		/// </summary>
		public Type CurrentComponent { get; private set; }

		#endregion

		#region Ctors

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

		#endregion

		#region Methods

		/// <summary>
		/// Load given component
		/// </summary>
		/// <param name="componentType"></param>
		public void Load(Type componentType)
		{
			this.CurrentComponent = componentType;
			
			this.messageBroker.Send(this.cefFrame, new DataMessage<FrameLoadMessageData>(MessageType.Load)
			{
				Data = new FrameLoadMessageData()
				{
					ComponentName = componentType.FullName,
					FrameName = this.Name
				}
			});
		}

		#endregion
	}
}