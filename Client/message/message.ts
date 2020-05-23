/**
 * Internal message interface
 */
import {MessageType} from "./MessageType";
import Page from "../page/Page";

interface IBaseMessage<TData = { [key: string]: any }>
{
	/**
	 * Internal message type
	 */
	messageType: MessageType;

	/**
	 * Internal data
	 */
	data: TData;

	/**
	 * User's data
	 */
	userData?: { [key: string]: any };
}

export type Message<TData = {[key: string]: any}> = { [key: string]: any } & IBaseMessage<TData>;

/**
 * Fetch data from C# codebase
 * @private
 * @param {{}} message
 * @param {number} [timeout] Timeout in ms
 */
export function _fetch<TResult>(message: Message, timeout?: number): Promise<TResult> & { cancel: () => void }
{
	let rejecter: (err: Error) => void;
	let requestId: string;
	let resolved = false;

	let promise: Promise<TResult> & { cancel: () => void } = new Promise((resolve, reject) => {
		rejecter = reject;

		if (timeout) {
			setTimeout(() => {
				if (!resolved) {
					resolved = true;
					reject(new Error("Promise timed out"));
				}
			}, timeout);
		}
		
		console.debug("Sending message:", message);

		requestId = window.cefQuery({
			request: JSON.stringify({
				"method": "POST",
				"url": null,
				"parameters": null,
				"postData": message,
			}),
			persistent: false,
			onSuccess: (response: any) => {
				resolved = true;
				resolve(response);
			},
			onFailure: (errorCode: number, errorMessage: string) => {
				resolved = true;
				reject({code: errorCode, message: errorMessage});
			}
		});
	}) as any;

	// Add cancel method to Promise object
	promise.cancel = () => {
		window.cefQueryCancel(requestId);
		resolved = true;
		rejecter(new Error("Promise canceled"))
	};

	return promise;
}

/**
 * Sync state with C# page ViewModel
 * @private
 * @param {Page} page
 * @param {number} [timeout] Timeout in ms
 */
export function syncState<TResult>(page: Page<any, any>): Promise<void>
{
	return _fetch({
		messageType: MessageType.SyncState,
		data: { 
			identifier: page.identifier,
			viewModel: page.props.viewModel
		}
	});
}

/**
 * Fetch data from C# codebase
 * @private
 * @param {string} page
 * @param {{}} params
 * @param {number} [timeout] Timeout in ms
 */
export function navigate<TResult>(page: string, params: { [key: string]: any }, timeout?: number): Promise<void>
{
	return _fetch({
		messageType: MessageType.Navigate,
		data: {page},
		userData: params
	});
}

// /**
//  * Fetch data from C# codebase
//  * @private
//  * @param {{}} data
//  * @param {number} [timeout] Timeout in ms
//  */
// export function fetch<TResult>("", data: { [key: string]: any }, timeout?: number): Promise<TResult> & { cancel: () => void }
// {
// 	return _fetch({
// 		messageType: MessageType.Navigate,
// 		data: { page },
// 		userData: data
// 	});
// }