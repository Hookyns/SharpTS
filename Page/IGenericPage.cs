using SharpTS.ViewModel;

namespace SharpTS.Page
{
	/// <summary>
	/// Generic page interface
	/// </summary>
	/// <typeparam name="TViewModel"></typeparam>
	public interface IGenericPage<out TViewModel> : IPage
		where TViewModel : class, IViewModel
	{
		
	}
}