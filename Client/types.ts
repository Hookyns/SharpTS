import {Message} from "./message/message";

/**
 * Base ViewModel interface
 */
export interface IViewModel
{

}

/**
 * Base page property interface
 */
export interface IPageProps
{
	viewModel?: IViewModel;
}

export type Operation = (message: Message) => Promise<void>;