using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace SharpTS.ViewModel
{
	public class ViewModelFactory
	{
		/// <summary>
		/// Proxy generator
		/// </summary>
		private readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

		/// <summary>
		/// Proxy generation options
		/// </summary>
		private readonly ProxyGenerationOptions proxyOptions = new ProxyGenerationOptions();

		/// <summary>
		/// Stored proxies which were already created
		/// </summary>
		private readonly ConcurrentDictionary<Type, Type> proxies = new ConcurrentDictionary<Type, Type>();

//		/// <summary>
//		/// Service scope
//		/// </summary>
//		private readonly IServiceScope serviceScope;

//		/// <summary>
//		/// Ctor
//		/// </summary>
//		public ViewModelFactory()
//		{
//			this.serviceScope = serviceScope;
//		}
			/// <summary>
			/// Create ViewModel instance
			/// </summary>
			/// <typeparam name="TViewModel"></typeparam>
			/// <returns></returns>
			public TViewModel Create<TViewModel>(IServiceScope serviceScope)
			where TViewModel : class, IViewModel
		{
			/*
			 * ServiceProvider used to keep ctor parameter-less
			 */

			if (typeof(TViewModel).GetInterfaces().Any(i => i == typeof(INotifyPropertyChanged)))
			{
				return this.CreateWithProxy<TViewModel>(serviceScope);
			}

			return (TViewModel)ActivatorUtilities.CreateInstance(serviceScope.ServiceProvider, typeof(TViewModel));
		}

		/// <summary>
		/// Create ViewModel instance with proxy for change detection
		/// </summary>
		/// <param name="serviceScope"></param>
		/// <typeparam name="TViewModel"></typeparam>
		/// <returns></returns>
		private TViewModel CreateWithProxy<TViewModel>(IServiceScope serviceScope) where TViewModel : class, IViewModel
		{
			Type vmType = typeof(TViewModel);
			Type proxiedVm;

			if (!this.proxies.TryGetValue(vmType, out proxiedVm))
			{
				proxiedVm = this.proxyGenerator.ProxyBuilder.CreateClassProxyType(vmType, new Type[0], this.proxyOptions);
				this.proxies.TryAdd(vmType, proxiedVm);
				
				// TODO: Projít daný typ a najít všechny property implementující INotifyPropertyChanged a předat to PropertyChangeInterceptor
			}
			
			return (TViewModel) ActivatorUtilities.CreateInstance(serviceScope.ServiceProvider, proxiedVm,
				new object[] { new IInterceptor[] {new PropertyChangeInterceptor()}});

			return (TViewModel)Activator.CreateInstance(proxiedVm, new object[] { new IInterceptor[] { new PropertyChangeInterceptor() } });

//			return (TViewModel)ActivatorUtilities.CreateInstance(serviceScope.ServiceProvider, proxiedVm);
		}
	}
}