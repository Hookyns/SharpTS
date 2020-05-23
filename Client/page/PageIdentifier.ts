export default class PageIdentifier
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
	 * Assembly qualified type name of page
	 */
	private readonly _typeName: string;

	/**
	 * Assembly qualified type name of page
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