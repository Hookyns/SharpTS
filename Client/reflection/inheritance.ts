/**
 * Check is type inherits from another type (static check)
 * @param type
 * @param parentType
 */
export function isSubclassOf(type: any, parentType: any) {
	let proto = type.prototype;

	while (proto.constructor && proto.constructor != Object) {
		if (proto.constructor == parentType) {
			return true;
		}

		proto = Object.getPrototypeOf(proto);
	}

	return false;
}