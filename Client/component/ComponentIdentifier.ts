export default class ComponentIdentifier
{
	/**
	 * Ctor
	 * @param typeName
	 * @param id
	 */
	constructor(typeName: string, id: string)
	{
		this._typeName = typeName;
		this._id = id;
	}

	/**
	 * Full type name of the component
	 */
	private readonly _typeName: string;

	/**
	 * Full type name of the component
	 */
	get typeName(): string
	{
		return this._typeName;
	}

	/**
	 * Id - C# Guid
	 */
	private readonly _id: string;

	/**
	 * Id - C# Guid
	 */
	get id(): string
	{
		return this._id;
	}
}