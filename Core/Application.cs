using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SharpTS.Core
{
    public abstract class Application
    {
        /// <summary>
        /// Application window
        /// </summary>
        internal Window Window { get; private set; }

        /// <summary>
        /// Name of 
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Application icon
        /// </summary>
        public string Icon { get; protected set; }

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
            window.OnNewFrame += this.NewFrame;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public virtual void ConfigureServices(IServiceCollection serviceCollection)
        {
        }

        /// <summary>
        /// Initialization method
        /// </summary>
        /// <returns></returns>
        public virtual Task Initialize()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// This method gets called by the runtime on application launch, after all configurations.
        /// </summary>
        /// <returns></returns>
        public virtual Task Loaded()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// New frame creation handler
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public virtual void NewFrame(Frame frame)
        {
        }

        /// <summary>
        /// Configuration handler
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public virtual void Configure(ApplicationBuilder applicationBuilder)
        {
        }
    }
}