using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SharpTS.Core
{
	public abstract class Application
	{
		/// <summary>
		/// Name of 
		/// </summary>
		public string Name { get; protected set; }
		
		/// <summary>
		/// Application window
		/// </summary>
		public Window Window { get; private set; }
		
		/// <summary>
		/// Internal ctor
		/// </summary>
		protected Application()
		{
		}

		/// <summary>
		/// Store window instance
		/// </summary>
		/// <param name="window"></param>
		internal void SetWindow(Window window)
		{
			this.Window = window;
		}
		
		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// <param name="serviceCollection"></param>
		/// <returns></returns>
		public virtual Task ConfiguraceServices(IServiceCollection serviceCollection)
		{
			return Task.CompletedTask;
		}
		
		/// <summary>
		/// This method gets called by the runtime on application launch, after all configurations.
		/// </summary>
		/// <returns></returns>
		public virtual Task OnLaunched()
		{
			return Task.CompletedTask;
		}
	}
}