using System;

namespace SharpTS.Component
{
	/// <summary>
	/// Component interface
	/// </summary>
	public interface IComponent : IDisposable
	{
		/// <summary>
		/// Component identifier
		/// </summary>
		ComponentIdentifier Identifier { get; }
	}
}