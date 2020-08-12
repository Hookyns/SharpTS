/**
 * Internal message interface
 */
import {MessageType} from "./MessageType";
import Component     from "../component/Component";

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
 * Sync state with C# component's ViewModel
 * @private
 * @param {Component} component
 * @param {number} [timeout] Timeout in ms
 */
export function syncState<TResult>(component: Component<any, any>): Promise<void>
{
	return _fetch({
		messageType: MessageType.SyncState,
		data: { 
			identifier: component.identifier,
			viewModel: component.props.viewModel
		}
	});
}

/**
 * Request loading of another component
 * @private
 * @param {string} component
 * @param {{}} params
 * @param {number} [timeout] Timeout in ms
 */
export function load<TResult>(component: string, params: { [key: string]: any }, timeout?: number): Promise<void>
{
	return _fetch({
		messageType: MessageType.Load,
		data: {component: component},
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
// 		data: { component },
// 		userData: data
// 	});
// }