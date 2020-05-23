namespace SharpTS.Message
{
	/// <summary>
	/// DTO for base message with type and custom data
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	public class DataMessage<TData> : BaseMessage
	{
		/// <summary>
		/// Custom data to send
		/// </summary>
		public TData Data { get; set; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="messageType"></param>
		public DataMessage(MessageType messageType) : base(messageType)
		{
		}
	}
}