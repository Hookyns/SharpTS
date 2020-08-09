// import * as globby from "globby";
// import * as $path from "path";
//
// class BundlerBuilder
// {
// 	/**
// 	 * List of entry patterns
// 	 */
// 	private entries: Array<string> = [];
//
// 	/**
// 	 * List of page entry patterns
// 	 */
// 	private pageEntries: Array<string> = [];
//
// 	/**
// 	 * Base dir
// 	 */
// 	private baseDir: string;
//
// 	/**
// 	 * Base dir
// 	 * @param baseDir
// 	 */
// 	constructor(baseDir: string)
// 	{
// 		this.baseDir = baseDir;
// 	}
//
// 	/**
// 	 * Main entry file
// 	 * @param path
// 	 */
// 	public main(path: string): BundlerBuilder
// 	{
// 		this.entries.push(path);
// 		return this;
// 	}
//
// 	/**
// 	 * Page patterns
// 	 * @param paths
// 	 */
// 	public pages(...paths: Array<string>): BundlerBuilder
// 	{
// 		this.pageEntries.push(...paths);
// 		return this;
// 	}
//
// 	/**
// 	 * Get entry object
// 	 */
// 	public async getEntries(): Promise<{ [key: string]: Array<string> }>
// 	{
// 		const options = { cwd: this.baseDir };
//		
// 		let entries = globby(this.entries, options);
// 		let pageEntries = globby(this.pageEntries, options);
//
// 		return {
// 			// main: (await entries).concat(await pageEntries),
// 			main: await entries,
// 			pages: await pageEntries
// 		}
// 	}
// }
//
// export function bundle() {
// 	return new BundlerBuilder(module.parent ? $path.dirname(module.parent.filename) : process.cwd());
// }