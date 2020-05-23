using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.ViewModel;

namespace SharpTS.Page
{
	/// <summary>
	/// Internal page interface
	/// </summary>
	/// <typeparam name="TViewModel"></typeparam>
	internal interface IInternalPage<out TViewModel> : IGenericPage<TViewModel>
		where TViewModel : class, IViewModel
	{
		/// <summary>
		/// DI Service scope
		/// </summary>
		IServiceScope ServiceScope { get; set; }
		
		/// <summary>
		/// ViewModel factory
		/// </summary>
		ViewModelFactory ViewModelFactory { get; set; }
		
		/// <summary>
		/// Method called before user navigate to this page
		/// </summary>
		/// <param name="args"></param>
		Task OnNavigatedTo(NavigationEventArgs args);

		/// <summary>
		/// Method called before user leave this page
		/// </summary>
		Task OnNavigatedFrom();

		/// <summary>
		/// Creates ViewModel instance
		/// </summary>
		/// <returns></returns>
		Task CreateViewModel();
	}
}