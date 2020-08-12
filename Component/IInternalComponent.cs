using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.ViewModel;

namespace SharpTS.Component
{
	/// <summary>
	/// Internal component interface
	/// </summary>
	/// <typeparam name="TViewModel"></typeparam>
	internal interface IInternalComponent<out TViewModel> : IGenericComponent<TViewModel>
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
		/// Method called before user navigate to this component
		/// </summary>
		/// <param name="args"></param>
		Task Created(CreateEventArgs args);

		/// <summary>
		/// Method called when the component should be destroy
		/// </summary>
		Task Destroyed();

		/// <summary>
		/// Called on activation when the component is marked as keep-alive 
		/// </summary>
		/// <returns></returns>
		Task Activated();

		/// <summary>
		/// Called instead of Destroy when the component is marked as keep-alive
		/// </summary>
		/// <returns></returns>
		Task Deactivated();

		/// <summary>
		/// Creates ViewModel instance
		/// </summary>
		/// <returns></returns>
		Task CreateViewModel();
	}
}