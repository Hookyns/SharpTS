using SharpTS.ViewModel;

namespace SharpTS.Component
{
	/// <summary>
	/// Generic component interface
	/// </summary>
	/// <typeparam name="TViewModel"></typeparam>
	public interface IGenericComponent<out TViewModel> : IComponent
		where TViewModel : class, IViewModel
	{
		
	}
}