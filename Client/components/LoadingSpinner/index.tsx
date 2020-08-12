import * as React           from "react";
import * as ReactDOM        from "react-dom";
import clsx                 from "clsx";
import {StandardProperties} from "csstype";

import "./style.less";

type LoadingSpinnerProps = {
	overlay?: boolean,
	fullScreen?: boolean,
	classes?: {
		overlay?: string,
		spin?: string,
	},
	styles?: {
		overlay?: StandardProperties,
		spin?: StandardProperties,
	}
}

export default class LoadingSpinner extends React.Component<LoadingSpinnerProps>
{
	render()
	{
		const element = this.props.overlay
			? (
				<div 
					className={clsx("loading-spinner-overlay", this.props.classes?.overlay)}
					style={this.props.styles?.overlay}
				>
					<div 
						className={clsx("loading-spinner-spin", this.props.classes?.spin)}
						style={this.props.styles?.spin}
					>...</div>
				</div>
			)
			: (
				<div 
					className={clsx("loading-spinner-spin", this.props.classes?.spin)}
					style={this.props.styles?.spin}
				>...</div>
			);

		if (!this.props.fullScreen)
		{
			return element;
		}

		return ReactDOM.createPortal(element, document.body);
	}
}