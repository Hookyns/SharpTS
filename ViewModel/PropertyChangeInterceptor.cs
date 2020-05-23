using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace SharpTS.ViewModel
{
	/// <summary>
	/// Property change detecting interceptor
	/// </summary>
	public class PropertyChangeInterceptor : IInterceptor/*, INotifyPropertyChanged*/
	{
		public void Intercept(IInvocation invocation)
		{
			// If it's setter
			if (invocation.Method.IsSpecialName 
				&& invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
			{
				// Call setter
				invocation.Proceed();

				// Get property name
				string propertyName = invocation.Method.Name.Substring(4);

				// Base type implementing INotifyPropertyChanged
				Type type = this.GetINotifyPropertyChangedType(invocation.InvocationTarget.GetType());

				if (type != null)
				{
					// Get PropertyChanged event backing field
					FieldInfo backingField = type.GetField(
						nameof(INotifyPropertyChanged.PropertyChanged),
						BindingFlags.Instance | BindingFlags.NonPublic);
					PropertyChangedEventHandler delegateInstance =
						(PropertyChangedEventHandler) backingField.GetValue(invocation.InvocationTarget);
					
					// Invoke event
					delegateInstance.Invoke(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		/// <summary>
		/// Find last of the BaseType which implements INotifyPropertyChanged
		/// </summary>
		/// <returns></returns>
		private Type GetINotifyPropertyChangedType(Type type)
		{
			if (type.GetInterfaces().Any(i => i == typeof(INotifyPropertyChanged)))
			{
				var baseT = this.GetINotifyPropertyChangedType(type.BaseType);

				if (baseT != null)
				{
					return baseT;
				}
				
				return type;
			}

			return null;
		}
	}
}