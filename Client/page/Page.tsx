import * as React from "react";
import {syncState} from "../message/message";
import PageIdentifier from "./PageIdentifier";
import {IPageProps, IViewModel} from "../types";
import {isSubclassOf} from "../reflection/inheritance";

export default abstract class Page<TProps extends IPageProps, TState extends IViewModel> extends React.Component<TProps, TState>
{
	/**
	 * Page identifier
	 */
	private readonly _identifier: PageIdentifier;

	/**
	 * Page instance identifier
	 */
	public get identifier(): PageIdentifier
	{
		return this._identifier;
	}

	/**
	 * Static page type identifier
	 */
	public static get typeName(): string
	{
		return this.hasOwnProperty("_typeName") ? (this as any)._typeName : undefined;
	}

	/**
	 * Page ctor
	 * @param props
	 * @param pageIdentifier
	 */
	public constructor(props: TProps, pageIdentifier: PageIdentifier)
	{
		super(props);

		this._identifier = pageIdentifier;
		this.setState(props.viewModel || {});
		props.viewModel = undefined;
	}

	/**
	 * After component update - cuz of state sync with C# code
	 * @param prevProps
	 * @param prevState
	 * @param snapshot
	 */
	componentDidUpdate(prevProps: Readonly<TProps>, prevState: Readonly<TState>, snapshot?: any): void
	{
		// Send state to C#
		syncState(this);
	}

	/**
	 * Check if given type is Page
	 * @param type
	 */
	public static isPage(type: any): type is (typeof Page) {
		return type != null && isSubclassOf(type, Page);
	}
}