import * as React              from "react";
import {ReactNode}             from "react";
import Bootstrap               from "../bootstrap";
import {Message}               from "../message/message";
import {IFrameLoadMessageData} from "../message/messages/IFrameLoadMessageData";
import {MessageType}           from "../message/MessageType";
import Component               from "./Component";

interface State
{
	component: typeof Component | undefined;
}

interface Props
{
	/**
	 * Name of frame
	 * @description Allow access from C# code - find frame by name.
	 */
	name: string;

	// /**
	//  * Flag marking main frame
	//  * @description Only 
	//  */
	// main: boolean;
}

interface NewFrameMessage extends Message
{
	/**
	 * Name of new frame
	 */
	frameName: string;
}

/**
 * Frame component able to render component
 */
export default class Frame extends React.Component<Props, State>
{
	/**
	 * Collection with frames
	 */
	private static _frames: { [frameName: string]: Frame } = {};

	/**
	 * Get existing frames
	 */
	public static get frames(): { [frameName: string]: Frame }
	{
		return Object.assign({}, this._frames);
	}

	/**
	 * Frame name
	 */
	public get name()
	{
		return this.props.name;
	}

	/**
	 * Ctor
	 * @param props
	 */
	constructor(props: Readonly<Props>)
	{
		super(props);

		// Store new frame
		Frame.frames[this.props.name] = this;

		// Listen to navigate event
		// @ts-ignore
		Bootstrap.instance.messageBroker.on(MessageType.Load, message => this.onLoad(message));

		// Send information about new frame instance
		// @ts-ignore
		Bootstrap.instance.messageBroker.send({messageType: MessageType.NewFrame, name: props.name})
	}

	/**
	 * Load/render another component
	 * @param component
	 */
	load(component: typeof Component)
	{
		this.setState({
			component: component
		});
	}

	/**
	 * Render frame
	 */
	render(): ReactNode
	{
		if (this.state && Component.isComponent(this.state.component))
		{
			const Component = this.state.component;
			return (<Component/>);
		}

		if (this.props.children)
		{
			return this.props.children;
		}

		return <div/>;

		// Looks better but not good as default value. Empty frame should be empty frame. Let user fill it.
		// return <LoadingSpinner fullScreen={true} overlay={true}/>;
	}

	/**
	 * On load request event
	 * @param message
	 */
	private onLoad(message: Message<IFrameLoadMessageData>): Promise<void>
	{
		console.log(`Frame[name=${this.props.name}].onNavigate(): `, message);

		if (message.data.frameName == this.props.name)
		{
			let component = Bootstrap.instance.componentLoader.find(message.data.componentName);

			if (!component)
			{
				console.error(`Frame[name=${this.props.name}]: load of component '${message.data.componentName}' failed, component not found!`);
				return Promise.resolve();
			}

			this.load(component);
		}

		return Promise.resolve();
	}
}