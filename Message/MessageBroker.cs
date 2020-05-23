using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Chromely.CefGlue;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SharpTS.Message
{
	internal class MessageBroker
	{
		#region Delegates
		
		/// <summary>
		/// Operation delegate
		/// </summary>
		public delegate Task Operation(MessageEventArgs args);

		#endregion

		#region Fields
		
		/// <summary>
		/// Name of JS function which will take messages
		/// </summary>
		private const string ClientMeaaseBrokerTakeFunc = "SharpTS.messageBroker.take";

		/// <summary>
		/// Event listeners
		/// </summary>
		private readonly Dictionary<MessageType, List<Operation>> messageListeners =
			new Dictionary<MessageType, List<Operation>>();

		/// <summary>
		/// Serialization settings
		/// </summary>
		private static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings()
		{
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			},
			Formatting = Formatting.None
		};

		#endregion
		
		#region Properties
		
		/// <summary>
		/// Handler
		/// </summary>
		public InteropMessageHandler MessageHandler { get; private set; }

		#endregion
		
		/// <summary>
		/// Ctor
		/// </summary>
		public MessageBroker()
		{
			// List of fallback handlers used if no handlers registered
			this.messageListeners.Add(MessageType.DOMContentLoaded, new List<Operation>() {this.DOMContentLoaded});
			this.messageListeners.Add(MessageType.Navigate, new List<Operation>() {this.Navigate});
			this.messageListeners.Add(MessageType.SyncState, new List<Operation>() {this.SyncState});
			this.messageListeners.Add(MessageType.Fetch, new List<Operation>() {this.Fetch});

			this.PrepareInteropHandler();
		}

		/// <summary>
		/// Send message to client
		/// </summary>
		/// <param name="message"></param>
		/// <typeparam name="TMessage"></typeparam>
		internal void Send<TMessage>(TMessage message)
			where TMessage : BaseMessage
		{
			FrameHandler.GetMainFrame().ExecuteJavaScript(this.SendMessageScript(message), null, 0);
		}

		/// <summary>
		/// Register handler for message type
		/// </summary>
		/// <param name="messageType"></param>
		/// <param name="operation"></param>
		internal void On(MessageType messageType, Operation operation)
		{
			if (this.messageListeners.TryGetValue(messageType, out var listeners))
			{
				listeners.Add(operation);
				return;
			}

			this.messageListeners.Add(messageType, new List<Operation>()
			{
				operation
			});
		}

		#region Private methods

		/// <summary>
		/// Create script 
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private string SendMessageScript(object message)
		{
			string messageData = Newtonsoft.Json.JsonConvert.SerializeObject(message, SerializeSettings);
			return $"{ClientMeaaseBrokerTakeFunc}({messageData})";
		}

		/// <summary>
		/// Prepare Interop Message Handler
		/// </summary>
		/// <returns></returns>
		private void PrepareInteropHandler()
		{
			this.MessageHandler = new InteropMessageHandler();
			this.MessageHandler.OnMessage += this.HandlerOnMessage;
		}

		/// <summary>
		/// On interop message
		/// </summary>
		/// <param name="args"></param>
		private async void HandlerOnMessage(MessageEventArgs args)
		{
			if (this.messageListeners.TryGetValue(args.BaseMessage.MessageType, out var handlers))
			{
				// LIFO iteration
				for (int i = handlers.Count - 1; i >= 0 && !args.Served; i--)
				{
					await handlers[i].Invoke(args);
					if (args.Served) break;
				}
			}

			if (!args.Served)
			{
				string msg = $"No operation registered or no operation respond to message with type '{args.BaseMessage.MessageType}'";
				
				Trace.WriteLine(msg);
				args.Failure(-1, msg);
			}
		}

		#endregion

		#region Operations

		private Task DOMContentLoaded(MessageEventArgs args)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Fetch data event handler
		/// </summary>
		private Task Fetch(MessageEventArgs args)
		{
			args.Success("");
			return Task.CompletedTask;
		}

		/// <summary>
		/// Navigate event handler
		/// </summary>
		private Task Navigate(MessageEventArgs args)
		{
			args.Success("");
			return Task.CompletedTask;
		}

		/// <summary>
		/// Sync state event handler
		/// </summary>
		private Task SyncState(MessageEventArgs args)
		{
			args.Success("");
			return Task.CompletedTask;
		}

		#endregion
	}
}