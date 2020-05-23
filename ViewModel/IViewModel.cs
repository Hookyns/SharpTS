namespace SharpTS.ViewModel
{
	/// <summary>
	/// ViewModel interface
	/// </summary>
	public interface IViewModel
	{
		/// <summary>
		/// On ViewModel change event
		/// </summary>
		event BaseViewModel.ViewModelChangedHandler OnChange;
	}
}