namespace SharpTS.ViewModel
{
	/// <summary>
	/// ViewModel change event argument 
	/// </summary>
	public class ViewModelChangedArgs
	{
		/// <summary>
		/// Type of change
		/// </summary>
		public ChangeType Type { get; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="type"></param>
		public ViewModelChangedArgs(ChangeType type)
		{
			this.Type = type;
		}
	}
}