import Page from "./page/Page";
import PageLoader from "./page/PageLoader";
import * as React from "react";
import {render} from "react-dom";
import MessageBroker from "./message/MessageBroker";
import {MessageType} from "./message/MessageType";
import {IRootComponentProps} from "./infrastructure/IRootComponentProps";
import ApplicationContext from "./infrastructure/ApplicationContext";

/**
 * Application bootstrapping class
 * @description Only one Bootstrap can exists
 */
export default class Bootstrap
{
	/**
	 * Singleton instance
	 */
	private static _instance: Bootstrap | null = null;

	/**
	 * Root component
	 */
	private component: typeof React.Component | undefined;

	/**
	 * Container HTMLElement
	 */
	private containerNode: Node | undefined;

	/**
	 * Page loader
	 */
	private pageLoader: PageLoader = new PageLoader();

	/**
	 * Message broker
	 */
	private messageBroker = new MessageBroker();

	/**
	 * Returns singleton instance of Bootstrap
	 */
	public static get instance(): Bootstrap {
		if (!this._instance) {
			this._instance = Reflect.construct(Bootstrap, [], BootstrapActivator);
		}
		
		return this._instance!;
	}

	/**
	 * Ctor
	 */
	constructor()
	{
		if (new.target != BootstrapActivator) {
			throw new Error("Bootstrap.constructor() is private!");
		}
		
		this.registerGlobals();
		this.initDomLoaded();
	}

	/**
	 * Register root component
	 * @param component
	 */
	public rootComponent(component: typeof React.Component): Bootstrap
	{
		this.component = component;
		return this;
	}

	/**
	 * Set application container
	 * @param element
	 */
	public container(element: HTMLElement | null): Bootstrap
	{
		if (!element) {
			throw new Error("Bootstrap.container: value of parameter 'element' cannot be null");
		}

		this.containerNode = element;
		return this;
	}

	/**
	 * Register pages in app
	 * @param pages
	 */
	public pages(pages: Array<Page<any, any>>): Bootstrap
	{
		this.pageLoader.addSourceModules(pages);
		return this;
	}

	/**
	 * Run app
	 */
	public run()
	{
		if (!this.component) {
			throw new Error("Root component not specified! Use Bootstrap.rootComponent() method to set root component.");
		}

		// TODO: Doesn't work, maybe cuz of dependency hell (same package loaded twice)
		// if (!isSubclassOf(this.component, React.Component)) {
		// 	throw new Error("Registered root component does not extends React.Component!");
		// }

		if (!this.containerNode) {
			throw new Error("Application container not specifier! Use Bootstrap.container() method to set application container.");
		}

		if (!(this.containerNode instanceof Node)) {
			throw new Error("Registered container is not Node!");
		}

		// @ts-ignore
		render(React.createElement(this.component as any, this.getRootComponentProps()), this.containerNode);
	}

	/**
	 * Register global variables for interop
	 */
	private registerGlobals()
	{
		let lib = (window as any).SharpTS || ((window as any).SharpTS = {});
		lib.messageBroker = this.messageBroker;
	}

	/**
	 * Initialize things on DOM loaded
	 */
	private initDomLoaded()
	{
		window.addEventListener("DOMContentLoaded", () => {
			// Notify C# code that View's DOM is ready after app start
			this.messageBroker.send({
				messageType: MessageType.DOMContentLoaded,
				data: {}
			});
		});
	}

	/**
	 * Properties for root component
	 */
	private getRootComponentProps(): IRootComponentProps
	{
		const appContext = new ApplicationContext(this);

		return {
			applicationContext: appContext
		};
	}
}

class BootstrapActivator extends Bootstrap
{
}

// let instance: Bootstrap | null = null;
//
// /**
//  * Returns singleton instance of Bootstrap
//  */
// export function bootstrap(): Bootstrap {
// 	return instance || (instance = new Bootstrap());
// }