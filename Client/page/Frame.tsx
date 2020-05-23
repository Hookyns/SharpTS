import * as React                  from "react";
import {ReactNode}                 from "react";
import Bootstrap                   from "../bootstrap";
import {Message}                   from "../message/message";
import {IFrameNavigateMessageData} from "../message/messages";
import {MessageType}               from "../message/MessageType";
import Page                        from "./Page";

interface State
{
	page: typeof Page | undefined;
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
 * Frame component able to render pages
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
		Bootstrap.instance.messageBroker.on(MessageType.Navigate, message => this.onNavigate(message));

		// Send information about new frame instance
		// @ts-ignore
		Bootstrap.instance.messageBroker.send({messageType: MessageType.NewFrame, name: props.name })
	}

	/**
	 * Navigate to another page
	 * @param page
	 */
	navigate(page: typeof Page)
	{
		this.setState({
			page
		});
	}

	/**
	 * Render frame
	 */
	render(): ReactNode
	{
		if (this.state && Page.isPage(this.state.page)) {
			const PageComponent = this.state.page;
			return (<PageComponent/>);
		}

		if (this.props.children) {
			return this.props.children;
		}

		return <div/>;
		
		// Looks better but not good as default value. Empty frame should be empty frame. Let user fill it.
		// return <LoadingSpinner fullScreen={true} overlay={true}/>;
	}

	/**
	 * On navigate request event
	 * @param message
	 */
	private onNavigate(message: Message<IFrameNavigateMessageData>): Promise<void>
	{
		console.log(`Frame[name=${this.props.name}].onNavigate(): `, message);
		
		if (message.data.frameName == this.props.name) {
			// @ts-ignore
			let page = Bootstrap.instance.pageLoader.find(message.data.pageName);
			
			if (!page) {
				console.error(`Frame[name=${this.props.name}]: navigation to page '${message.data.pageName}' failed, page not found!`);
				return Promise.resolve();
			}
			
			this.navigate(page);
		}
		
		return Promise.resolve();
	}
}