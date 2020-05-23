namespace SharpTS.Message.Messages
{
	/// <summary>
	/// Object for navigation data
	/// </summary>
	public class FrameNavigateMessageData
	{
		/// <summary>
		/// Name of frame which should navigate
		/// </summary>
		public string FrameName { get; set; }
		
		/// <summary>
		/// Name of page which should be loaded
		/// </summary>
		public string PageName { get; set; }
	}
}