import * as React                    from "react";
import {syncState}                   from "../message/message";
import ComponentIdentifier           from "./ComponentIdentifier";
import {IComponentProps, IViewModel} from "../types";
import {isSubclassOf}                from "../reflection/inheritance";

export default abstract class Component<TProps extends IComponentProps, TState extends IViewModel> extends React.Component<TProps, TState>
{
	/**
	 * Component identifier
	 */
	private readonly _identifier: ComponentIdentifier;

	/**
	 * Component instance identifier
	 */
	public get identifier(): ComponentIdentifier
	{
		return this._identifier;
	}

	/**
	 * Static component type identifier
	 */
	public static get typeName(): string
	{
		return this.hasOwnProperty("_typeName") ? (this as any)._typeName : undefined;
	}

	/**
	 * Component ctor
	 * @param props
	 * @param componentIdentifier
	 */
	public constructor(props: TProps, componentIdentifier: ComponentIdentifier)
	{
		super(props);

		this._identifier = componentIdentifier;
		this.state = (props.viewModel ?? ({} as IViewModel)) as any;
		// props.viewModel = undefined; // NOTE: Idk why it was here. Maybe to prevent updating state directly cuz viewmodel is the state after assignment one line up; but React prevent edits on state, it's imutable.
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
	 * Check if given type is Component
	 * @param type
	 */
	public static isComponent(type: any): type is (typeof Component)
	{
		return type != null && isSubclassOf(type, Component);
	}
}