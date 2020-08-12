using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpTS.Message;

namespace SharpTS.Core
{
	/// <summary>
	/// Class representing application window which hold frames with rendered content
	/// </summary>
	internal class Window
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
		public Window(MessageBroker messageBroker)
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
				// TODO: What to do when reloaded? Dispose old frame and init new again? Everything will work as new load but state wil be lost.
				string frameName = args.PostData.Value<string>("name");

				if (string.IsNullOrWhiteSpace(frameName))
				{
					args.Failure(-1, "Frame name undefined");
					return Task.CompletedTask;
				}

				Frame frame;
				bool newFrame = false;
				
				// Exsting frame
				if (this.Frames.TryGetValue(frameName, out frame))
				{
					frame.Load(frame.CurrentComponent);
				}
				// New frame
				else
				{
					frame = new Frame(frameName, args.CefFrame, this.messageBroker);
					newFrame = true;
				
					// Add to collection of existing frames
					this.Frames.Add(frameName, frame);
				}
				
				
				// Confirm success
				args.Success();
				
				// Invoke handlers
				if (newFrame)
				{
					this.OnNewFrame?.Invoke(frame);
				}
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