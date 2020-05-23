namespace SharpTS.ViewModel
{
	public abstract class BaseViewModel : IViewModel
	{
		/// <summary>
		/// View model change event argument
		/// </summary>
		/// <param name="arg"></param>
		public delegate void ViewModelChangedHandler(ViewModelChangedArgs arg);

		/// <summary>
		/// On ViewModel change event
		/// </summary>
		public event ViewModelChangedHandler OnChange;
	}
}