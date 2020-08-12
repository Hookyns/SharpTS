/**
 * Object for load data
 */
export interface IFrameLoadMessageData {

	/**
	 * Name of frame which should navigate
	 */
	frameName: string;

	/**
	 * Name of component which should be loaded
	 */
	componentName: string;
}