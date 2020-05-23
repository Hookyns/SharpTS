using System;
using Microsoft.Extensions.DependencyInjection;

namespace SharpTS.DI
{
	/// <summary>
	/// Class allowing usage of Lazy<> for DI
	/// </summary>
	/// <typeparam name="TService"></typeparam>
	internal class LazyService<TService> : Lazy<TService>
		where TService : class
	{
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="serviceProvider"></param>
		public LazyService(IServiceProvider serviceProvider)
			: base(serviceProvider.GetRequiredService<TService>)
		{
		}
	}
}