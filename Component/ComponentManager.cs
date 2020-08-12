using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SharpTS.ViewModel;

namespace SharpTS.Component
{
	public class ComponentManager
	{
		#region Fields

		/// <summary>
		/// Component factory
		/// </summary>
		private readonly ComponentFactory componentFactory;

		/// <summary>
		/// Dictionary holding existing components instances
		/// </summary>
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<ComponentIdentifier, IInternalComponent<IViewModel>>>
			components = new ConcurrentDictionary<Type, ConcurrentDictionary<ComponentIdentifier, IInternalComponent<IViewModel>>>();

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="componentFactory"></param>
		public ComponentManager(ComponentFactory componentFactory)
		{
			this.componentFactory = componentFactory;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Create new component instance
		/// </summary>
		/// <param name="componentIdentifier"></param>
		/// <returns></returns>
		public IGenericComponent<IViewModel> CreateNew(ComponentIdentifier componentIdentifier)
		{
			var component = this.componentFactory.Create(componentIdentifier.ComponentType);
			this.StoreComponent(component);
			return component;
		}

		/// <summary>
		/// Get existing component instance or create new
		/// </summary>
		/// <param name="componentIdentifier"></param>
		/// <returns></returns>
		public IGenericComponent<IViewModel> Get(ComponentIdentifier componentIdentifier)
		{
			return this.Find(componentIdentifier) ?? this.CreateNew(componentIdentifier);
		}

		/// <summary>
		/// Find existing instance
		/// </summary>
		/// <param name="componentIdentifier"></param>
		/// <returns></returns>
		public IGenericComponent<IViewModel> Find(ComponentIdentifier componentIdentifier)
		{
			if (this.components.TryGetValue(componentIdentifier.ComponentType, out var instancesDist))
			{
				if (instancesDist.TryGetValue(componentIdentifier, out var component))
				{
					return component;
				}
			}

			return null;
		}

		/// <summary>
		/// List existing component instances
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IGenericComponent<IViewModel>> ListComponents()
		{
			return this.components.SelectMany(type => type.Value.Select(instance => instance.Value));
		}

		/// <summary>
		/// Destroy component - remove references and dispose
		/// </summary>
		/// <param name="componentIdentifier"></param>
		public void Destroy(ComponentIdentifier componentIdentifier)
		{
			if (this.components.TryGetValue(componentIdentifier.ComponentType, out var instancesDist))
			{
				IInternalComponent<IViewModel> component;
				if (instancesDist.TryRemove(componentIdentifier, out component))
				{
					component.Destroyed().ContinueWith(_ =>
					{
						component.Dispose();
					});
				}
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Store new component
		/// </summary>
		/// <param name="component"></param>
		private void StoreComponent(IGenericComponent<IViewModel> component)
		{
			if (!this.components.TryGetValue(component.Identifier.ComponentType, out var instancesDict))
			{
				instancesDict = new ConcurrentDictionary<ComponentIdentifier, IInternalComponent<IViewModel>>();
			}

			instancesDict.TryAdd(component.Identifier, (IInternalComponent<IViewModel>)component);
		}

		#endregion
	}
}