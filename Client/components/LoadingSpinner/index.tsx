import * as React from "react";
import * as ReactDOM from "react-dom";

import "./style.less";

type LoadingSpinnerProps = {
	overlay?: boolean,
	fullScreen?: boolean,
}

export default class LoadingSpinner extends React.Component<LoadingSpinnerProps>
{
	render()
	{
		const element = this.props.overlay
			? (
				<div className="loading-spinner-overlay">
					<div className="loading-spinner-spin">...</div>
				</div>
			)
			: (
				<div className="loading-spinner-spin">...</div>
			);

		if (!this.props.fullScreen) {
			return element;
		}

		return ReactDOM.createPortal(element, document.body);
	}
}