using System;
using SharpTS.ViewModel;

namespace SharpTS.Page
{
	/// <summary>
	/// Page identification
	/// </summary>
	public class PageIdentifier
	{
		#region Fields
		
		private Type pageType;

		#endregion
		
		#region Properties
		
		/// <summary>
		/// Name of page
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
		/// Page's assembly qualified name
		/// </summary>
		public string TypeName { get; }
		
		/// <summary>
		/// Type of page
		/// </summary>
		public Type PageType => this.pageType ?? (this.pageType = Type.GetType(this.TypeName));

		#endregion

		#region Ctors

		public PageIdentifier(string typeName, string name = null)
		{
			this.TypeName = typeName;
			this.Name = name ?? "Unnamed page";
		}

		#endregion
		
		#region Methods
		
		/// <summary>
		/// Get Identifier of Page
		/// </summary>
		/// <param name="page"></param>
		/// <typeparam name="TViewModel"></typeparam>
		/// <returns></returns>
		public static PageIdentifier GetIdentifier<TViewModel>(Page<TViewModel> page)
			where TViewModel : class, IViewModel
		{
			return new PageIdentifier(page.GetType().AssemblyQualifiedName, page.GetType().Name)
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

			PageIdentifier objIdentifier = obj as PageIdentifier;

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