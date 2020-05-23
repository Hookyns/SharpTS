namespace SharpTS.ViewModel
{
	public enum ChangeType
	{
		/// <summary>
		/// New value set
		/// </summary>
		Assign,
		
		/// <summary>
		/// Value updated without reaasign (nested object updated)
		/// </summary>
		Update
	}
}