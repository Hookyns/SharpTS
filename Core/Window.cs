using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SharpTS.Message;

namespace SharpTS.Core
{
	/// <summary>
	/// Class representing application window which hold frames with rendered content
	/// </summary>
	public class Window
	{
		#region Delegates

		/// <summary>
		/// New frame event handler
		/// </summary>
		public delegate void NewFrameHandler(Frame frame);
		
		#endregion
		
		#region Events

		/// <summary>
		/// Event triggered when new frame in View created
		/// </summary>
		public event NewFrameHandler OnNewFrame;
		
		#endregion
		
		#region Fields

		/// <summary>
		/// Message broker instance
		/// </summary>
		private readonly MessageBroker messageBroker;

		#endregion

		#region Properties

		/// <summary>
		/// Frames in window
		/// </summary>
		public IDictionary<string, Frame> Frames { get; } = new Dictionary<string, Frame>();

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="messageBroker"></param>
		internal Window(MessageBroker messageBroker)
		{
			this.messageBroker = messageBroker;

			this.Initialize();
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Initialization method
		/// </summary>
		private void Initialize()
		{
			this.messageBroker.On(MessageType.NewFrame, this.NewFrame);
		}

		/// <summary>
		/// Handler listening for new frame
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private Task NewFrame(MessageEventArgs args)
		{
			try
			{
				string frameName = args.PostData.Value<string>("name");

				if (string.IsNullOrWhiteSpace(frameName))
				{
					args.Failure(-1, "Frame name undefined");
					return Task.CompletedTask;
				}
				
				Frame frame = new Frame(frameName, args.CefFrame, this.messageBroker);
				
				// Add to collection of existing frames
				this.Frames.Add(frameName, frame);
				
				// Confirm success
				args.Success();
				
				// Invoke handlers
				this.OnNewFrame?.Invoke(frame);
			}
			catch (Exception ex)
			{
				args.Failure(-1, ex.Message);
			}

			return Task.CompletedTask;
		}

		#endregion
	}
}