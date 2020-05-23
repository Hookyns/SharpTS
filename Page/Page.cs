using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.ViewModel;

namespace SharpTS.Page
{
	/// <summary>
	/// Base class for Page
	/// </summary>
	/// <typeparam name="TViewModel"></typeparam>
	public abstract class Page<TViewModel> : IInternalPage<TViewModel>
		where TViewModel : class, IViewModel
	{
		#region Fields

		/// <summary>
		/// Page identifier
		/// </summary>
		private PageIdentifier identifier;

		#endregion
		
		#region Properties
		
		/// <summary>
		/// Page View-Model
		/// </summary>
		protected internal TViewModel ViewModel { get; internal set; }

		/// <summary>
		/// Page identifier
		/// </summary>
		public PageIdentifier Identifier => this.identifier ?? (this.identifier = PageIdentifier.GetIdentifier(this));

		/// <summary>
		/// DI Service scope
		/// </summary>
		internal IServiceScope ServiceScope { get; set; }

		/// <summary>
		/// ViewModel factory
		/// </summary>
		internal ViewModelFactory ViewModelFactory { get; set; }

		/// <summary>
		/// DI Service scope
		/// </summary>
		IServiceScope IInternalPage<TViewModel>.ServiceScope
		{
			get => this.ServiceScope;
			set => this.ServiceScope = value;
		}

		/// <summary>
		/// ViewModel factory
		/// </summary>
		ViewModelFactory IInternalPage<TViewModel>.ViewModelFactory
		{
			get => this.ViewModelFactory;
			set => this.ViewModelFactory = value;
		}

		#endregion
		
		#region Methods
		
		/// <summary>
		/// Method called before user navigate to this page
		/// </summary>
		/// <param name="args"></param>
		protected virtual async Task OnNavigatedTo(NavigationEventArgs args)
		{
		}

		/// <summary>
		/// Method called before user leave this page
		/// </summary>
		protected virtual async Task OnNavigatedFrom()
		{
			
		}

		/// <summary>
		/// Creates ViewModel instance
		/// </summary>
		/// <returns></returns>
		protected virtual async Task CreateViewModel()
		{
			this.ViewModel = this.ViewModelFactory.Create<TViewModel>(this.ServiceScope);
		}
		
		
		/// <summary>
		/// Method called before user navigate to this page
		/// </summary>
		/// <param name="args"></param>
		async Task IInternalPage<TViewModel>.OnNavigatedTo(NavigationEventArgs args)
		{
			await OnNavigatedTo(args);
		}

		/// <summary>
		/// Method called before user leave this page
		/// </summary>
		async Task IInternalPage<TViewModel>.OnNavigatedFrom()
		{
			await this.OnNavigatedFrom();
		}

		/// <summary>
		/// Creates ViewModel instance
		/// </summary>
		/// <returns></returns>
		async Task IInternalPage<TViewModel>.CreateViewModel()
		{
			await this.CreateViewModel();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public virtual void Dispose()
		{
			// Dispose service scope
			this.ServiceScope?.Dispose();
			
			// Check if ViewModel is IDisposable; Dispose if it is
			IDisposable vmDisposable = this.ViewModel as IDisposable;
			vmDisposable?.Dispose();
		}
		
		#endregion
	}
}