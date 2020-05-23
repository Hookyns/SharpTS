import {MessageType} from "./MessageType";
import {_fetch, Message} from "./message";
import {Operation} from "../types";

export default class MessageBroker
{
	/**
	 * Map of registered operations to message types
	 */
	private messageListeners: Map<MessageType, Array<Operation>> = new Map();

	// noinspection JSUnusedGlobalSymbols
	/**
	 * Put message to MessageBroker
	 * @description Called by C# code to put message to queue of message broker.
	 * @private Called by C# code
	 * @param message
	 */
	public take(message: Message)
	{
		let operations: Array<Operation> = this.messageListeners.get(message.messageType) || [];

		(async () => {
			for (let i = operations.length - 1; i >= 0; i--) {
				let operation = operations[i];
				await operation(message);
			}
		})();
	}

	/**
	 * Send message to C#
	 * @param message
	 * @param timeout
	 */
	public send<TResult>(message: Message, timeout?: number): Promise<TResult> & { cancel: () => void }
	{
		return _fetch(message, timeout);
	}

	/// <summary>
	/// Register handler for message type
	/// </summary>
	/// <param name="messageType"></param>
	/// <param name="operation"></param>
	public on(messageType: MessageType, operation: Operation)
	{
		let listeners = this.messageListeners.get(messageType);
		
		if (!listeners) {
			listeners = [];
			this.messageListeners.set(messageType, listeners);
		}
		
		listeners.push(operation);
	}
}