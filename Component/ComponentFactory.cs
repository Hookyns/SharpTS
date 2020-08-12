using System;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.ViewModel;

namespace SharpTS.Component
{
	/// <summary>
	/// Component factory
	/// </summary>
	public class ComponentFactory
	{
		/// <summary>
		/// Service provider
		/// </summary>
		private readonly IServiceProvider serviceProvider;

		/// <summary>
		/// ViewModel factory
		/// </summary>
		private readonly ViewModelFactory viewModelFactory;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <param name="viewModelFactory"></param>
		public ComponentFactory(IServiceProvider serviceProvider, ViewModelFactory viewModelFactory)
		{
			this.serviceProvider = serviceProvider;
			this.viewModelFactory = viewModelFactory;
		}

		/// <summary>
		/// Create component
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <returns></returns>
		public TComponent Create<TComponent>()
			where TComponent : Component<IViewModel>
		{
			return (TComponent)this.Create(typeof(TComponent));
		}

		/// <summary>
		/// Create component
		/// </summary>
		/// <returns></returns>
		public IGenericComponent<IViewModel> Create(Type componentType)
		{
			// New scope, disposed from within Component
			IServiceScope scope = this.serviceProvider.CreateScope();

			// Resolve Component instance
			IInternalComponent<IViewModel> component = (IInternalComponent<IViewModel>)scope.ServiceProvider.GetService(componentType);

			// Set internal dependencies scope; to keep Component's ctor parameter-less
			component.ServiceScope = scope;
			component.ViewModelFactory = this.viewModelFactory;

			return component;
		}
	}
}