using System;
using SharpTS.ViewModel;

namespace SharpTS.Component
{
	/// <summary>
	/// Component identification
	/// </summary>
	public class ComponentIdentifier
	{
		#region Fields
		
		private Type componentType;

		#endregion
		
		#region Properties
		
		/// <summary>
		/// Name of component
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Instance identifier
		/// </summary>
		/// <remarks>
		/// Generated for instances, only by <see cref="GetIdentifier{TViewModel}"/>.
		/// </remarks>
		public Guid? Id { get; private set; }

		/// <summary>
		/// Component's full name
		/// </summary>
		public string TypeName { get; }
		
		/// <summary>
		/// Type of component
		/// </summary>
		public Type ComponentType => this.componentType ?? (this.componentType = Type.GetType(this.TypeName));

		#endregion

		#region Ctors

		public ComponentIdentifier(string typeName, string name = null)
		{
			this.TypeName = typeName;
			this.Name = name ?? "Unnamed component";
		}

		#endregion
		
		#region Methods
		
		/// <summary>
		/// Get Identifier of the Component
		/// </summary>
		/// <param name="component"></param>
		/// <typeparam name="TViewModel"></typeparam>
		/// <returns></returns>
		public static ComponentIdentifier GetIdentifier<TViewModel>(Component<TViewModel> component)
			where TViewModel : class, IViewModel
		{
			return new ComponentIdentifier(component.GetType().AssemblyQualifiedName, component.GetType().Name)
			{
				Id = Guid.NewGuid()
			};
		}

		/// <summary>
		/// Check if identifiers are equal
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (!this.Id.HasValue)
			{
				return false;
			}

			ComponentIdentifier objIdentifier = obj as ComponentIdentifier;

			if (objIdentifier == null)
			{
				return false;
			}
			
			return this.Id == objIdentifier.Id;
		}

		/// <summary>
		/// Get identifier HashCode
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode, BaseObjectGetHashCodeCallInGetHashCode
			return this.Id?.GetHashCode() ?? base.GetHashCode();
		}
		
		#endregion
	}
}