export enum MessageType {
	/**
	 * Send notification about DOMContentLoaded
	 */
	DOMContentLoaded,
	
	/**
	 * Load another component
	 */
	Load,

	/**
	 * Sync state with C# component ViewModel
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