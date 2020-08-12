import Component      from "./Component";
import {isSubclassOf} from "../reflection/inheritance";

export default class ComponentLoader {
	/**
	 * Component modules
	 */
	private readonly components: Map<string, typeof Component> = new Map();

	/**
	 * Add modules to loader. Modules gonna be filtered only to those which extends Component.
	 * @param modules
	 */
	public addSourceModules(modules: Array<any>) {
		// m.default stands for default export from module
		let modulesDefaultExports = modules.map(module => module.default).filter(m => isSubclassOf(m, Component));
		
		debugger;
		
		for (let modulesDefaultExport of modulesDefaultExports) {
			console.log(modulesDefaultExport);
			this.components.set(modulesDefaultExport.typeName, modulesDefaultExport);
		}
	}

	/**
	 * Find Component by type
	 * @param typeName
	 */
	public find(typeName: string): typeof Component | undefined {
		console.log(typeName, this.components);
		
		return this.components.get(typeName);
	}
}