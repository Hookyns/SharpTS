import {Message} from "./message/message";

/**
 * Base ViewModel interface
 */
export interface IViewModel
{

}

/**
 * Base component property interface
 */
export interface IComponentProps
{
	viewModel?: IViewModel;
}

export type Operation = (message: Message) => Promise<void>;