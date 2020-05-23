using System;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.ViewModel;

namespace SharpTS.Page
{
	/// <summary>
	/// Page factory
	/// </summary>
	public class PageFactory
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
		public PageFactory(IServiceProvider serviceProvider, ViewModelFactory viewModelFactory)
		{
			this.serviceProvider = serviceProvider;
			this.viewModelFactory = viewModelFactory;
		}

		/// <summary>
		/// Create page
		/// </summary>
		/// <typeparam name="TPage"></typeparam>
		/// <returns></returns>
		public TPage Create<TPage>()
			where TPage : Page<IViewModel>
		{
			return (TPage)this.Create(typeof(TPage));
		}

		/// <summary>
		/// Create page
		/// </summary>
		/// <returns></returns>
		public IGenericPage<IViewModel> Create(Type pageType)
		{
			// New scope, disposed from within Page
			IServiceScope scope = this.serviceProvider.CreateScope();

			// Resolve Page instance
			IInternalPage<IViewModel> page = (IInternalPage<IViewModel>)scope.ServiceProvider.GetService(pageType);

			// Set internal dependencies scope; to keep Page's ctor parameter-less
			page.ServiceScope = scope;
			page.ViewModelFactory = this.viewModelFactory;

			return page;
		}
	}
}