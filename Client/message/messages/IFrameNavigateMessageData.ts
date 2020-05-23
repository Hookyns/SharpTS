/**
 * Object for navigation data
 */
export interface IFrameNavigateMessageData {

	/**
	 * Name of frame which should navigate
	 */
	frameName: string;

	/**
	 * Name of page which should be loaded
	 */
	pageName: string;
}