using System;

namespace SharpTS.Page
{
	/// <summary>
	/// Page interface
	/// </summary>
	public interface IPage : IDisposable
	{
		/// <summary>
		/// Page identifier
		/// </summary>
		PageIdentifier Identifier { get; }
	}
}