namespace SharpTS.Message
{
	public class BaseMessage
	{
		/// <summary>
		/// Message type
		/// </summary>
		public MessageType MessageType { get; set; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="messageType"></param>
		public BaseMessage(MessageType messageType)
		{
			this.MessageType = messageType;
		}
	}
}