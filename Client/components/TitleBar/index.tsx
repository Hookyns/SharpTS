import * as React           from "react";
import * as ReactDOM        from "react-dom";
import clsx                 from "clsx";
import {StandardProperties} from "csstype";

import "./style.less";

type Props = {
	classes?: {
		titleBar?: string,
		strip?: string,
	},
	styles?: {
		titleBar?: StandardProperties,
		strip?: StandardProperties,
	}
}

export default class TitleBar extends React.Component<Props>
{
	render()
	{
		return <div 
			className={clsx("title-bar", this.props.classes?.titleBar)}
			style={this.props.styles?.titleBar}
		>
			<div 
				className={clsx("strip", this.props.classes?.strip)} 
				style={this.props.styles?.strip}
			/>
			{this.props.children}
		</div>;
	}
}