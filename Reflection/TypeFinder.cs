using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpTS.Reflection
{
	/// <summary>
	/// Static class with type finding methods
	/// </summary>
	public static class TypeFinder
	{
		#region Methods

		/// <summary>
		/// Returns collection of derived types
		/// </summary>
		/// <param name="type"></param>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public static ICollection<Type> GetSubclassesOf(Type type, ICollection<Assembly> assemblies = null)
		{
			if (assemblies == null)
			{
				assemblies = AppDomain.CurrentDomain.GetAssemblies();
			}

			return assemblies.SelectMany(a => a.GetTypes()).Where(t => IsSubclassOf(type, t) && t != type).ToList();
		}
		
		#endregion
		
		#region Private methods
		
		/// <summary>
		/// Check if class's type is subclass of another class's type
		/// </summary>
		/// <param name="baseType"></param>
		/// <param name="typeToCheck"></param>
		/// <returns></returns>
		private static bool IsSubclassOf(Type baseType, Type typeToCheck) {
			while (typeToCheck != null && typeToCheck != typeof(object)) {
				Type type = typeToCheck.IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
				
				if (baseType == type) {
					return true;
				}
				
				typeToCheck = typeToCheck.BaseType;
			}
			
			return false;
		}
		
		#endregion
	}
}