namespace SharpTS.Message.Messages
{
	/// <summary>
	/// Object for navigation data
	/// </summary>
	public class FrameLoadMessageData
	{
		/// <summary>
		/// Name of frame which should navigate
		/// </summary>
		public string FrameName { get; set; }
		
		/// <summary>
		/// Name of component which should be loaded
		/// </summary>
		public string ComponentName { get; set; }
	}
}