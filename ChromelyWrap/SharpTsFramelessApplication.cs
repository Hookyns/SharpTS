using Chromely;
using Microsoft.Extensions.DependencyInjection;

namespace SharpTS.ChromelyWrap
{
	public sealed class SharpTsFramelessApplication : ChromelyFramelessApp
	{
		public override void ConfigureServices(ServiceCollection services)
		{
			base.ConfigureServices(services);

		}
	}
}