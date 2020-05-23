export enum MessageType {
	/**
	 * Send notification about DOMContentLoaded
	 */
	DOMContentLoaded,
	
	/**
	 * Navigate to another page
	 */
	Navigate,

	/**
	 * Sync state with C# page ViewModel
	 */
	SyncState,

	/**
	 * Fetch data
	 */
	Fetch,

	/**
	 * Notify C# about new frame
	 */
	NewFrame
}