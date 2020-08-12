namespace SharpTS.Message
{
	/// <summary>
	/// Message type enum
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// Send from client to server, notifying about DOMContentLoaded
		/// </summary>
		DOMContentLoaded = 0,
		
		/// <summary>
		/// Load another component
		/// </summary>
		Load,
		
		/// <summary>
		/// Sync state of ViewModel
		/// </summary>
		SyncState,

		/// <summary>
		/// Fetch data
		/// </summary>
		Fetch,
		
		/// <summary>
		/// Notify about new frame
		/// </summary>
		NewFrame
	}
}