import Page from "./Page";
import {isSubclassOf} from "../reflection/inheritance";

export default class PageLoader {
	/**
	 * Page modules
	 */
	private readonly pages: Map<string, typeof Page> = new Map();

	/**
	 * Add modules to loader. Modules gonna be filtered only to those which extends Page.
	 * @param modules
	 */
	public addSourceModules(modules: Array<any>) {
		// m.default stands for default export from module
		let pageModules = modules.map(module => module.default).filter(m => isSubclassOf(m, Page));
		
		for (let page of pageModules) {
			this.pages.set(page.typeName, page);
		}
	}

	/**
	 * Find Page by type
	 * @param typeName
	 */
	public find(typeName: string): typeof Page | undefined {
		console.log(typeName, this.pages);
		
		return this.pages.get(typeName);
	}
}