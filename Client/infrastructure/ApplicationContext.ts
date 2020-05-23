import Bootstrap from "../bootstrap";

export default class ApplicationContext
{
	/**
	 * Bootstrap instance
	 */
	private _bootstrap: Bootstrap;

	/**
	 * Ctor
	 * @param bootstrap
	 */
	constructor(bootstrap: Bootstrap)
	{
		this._bootstrap = bootstrap;
	}
}