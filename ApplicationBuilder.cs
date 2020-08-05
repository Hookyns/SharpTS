using System;
using Chromely.CefGlue;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.ChromelyProxy;
using SharpTS.Core;
using SharpTS.DI;
using SharpTS.Message;
using SharpTS.Page;
using SharpTS.Reflection;
using SharpTS.ViewModel;

namespace SharpTS
{
	/// <summary>
	/// Application framework class
	/// </summary>
	public class ApplicationBuilder
	{
		#region Fields

		/// <summary>
		/// Start-up file
		/// </summary>
		const string StartUrl = "local://UI/dist/index.html";

		/// <summary>
		/// Static locking object
		/// </summary>
		private static object @lock = new object();
		
		/// <summary>
		/// Application singleton instance
		/// </summary>
		private static ApplicationBuilder instance;

		/// <summary>
		/// Application instance
		/// </summary>
		private Application application;

		/// <summary>
		/// DI service collection
		/// </summary>
		private readonly ServiceCollection serviceCollection = new ServiceCollection();

		/// <summary>
		/// DI service provider
		/// </summary>
		private ServiceProvider serviceProvider;
		
		/// <summary>
		/// Interop message broker
		/// </summary>
		private readonly MessageBroker messageBroker = new MessageBroker();

		#endregion

		#region Properties

		/// <summary>
		/// Window instance
		/// </summary>
		public AppWindow AppWindow { get; private set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="application">Application name</param>
		private ApplicationBuilder(Application application)
		{
			this.application = application;

			this.Initialize();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Create builder over given application
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static ApplicationBuilder Build(Application application)
		{
			if (application == null)
			{
				throw new ArgumentNullException(nameof(application));
			}

			lock (@lock)
			{
				if (instance != null)
				{
					throw new InvalidOperationException("You can build only one application!");
				}
				
				instance = new ApplicationBuilder(application);
			}

			return instance;
		}

		/// <summary>
		/// Start application
		/// </summary>
		public int Start(string startUrl = null)
		{
			this.AppWindow.Configure(config =>
			{
				config
					.WithDefaultSubprocess()
					.WithHostTitle(this.application.Name)
					.WithSilentCefBinariesLoading(true)
					.UseDefaultResourceSchemeHandler("local", string.Empty)
					.WithStartUrl(string.IsNullOrWhiteSpace(startUrl) ? StartUrl : startUrl)
					.RegisterMessageRouterHandler(this.messageBroker.MessageHandler)
					;
			});

			return this.AppWindow.Create();
		}

		public ApplicationBuilder Debug()
		{
			this.AppWindow.Debug();
			return this;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Initialize application
		/// </summary>
		private void Initialize()
		{
			this.InitializeApplication();

			this.RegisterInternalServices();

			// Let application configure services
			this.application.ConfiguraceServices(this.serviceCollection);
			
			// Build provider from collection
			this.serviceProvider = this.serviceCollection.BuildServiceProvider();

			// Create window
			this.AppWindow = new AppWindow();

			this.messageBroker.On(MessageType.DOMContentLoaded, async (arg) =>
			{
				await this.application.OnLaunched();
				arg.Success(null);
			});
		}

		/// <summary>
		/// Initialize application
		/// </summary>
		private void InitializeApplication()
		{
			// Create Window
			Window window = new Window(this.messageBroker);
			this.application.SetWindow(window);
		}

		/// <summary>
		/// Register internal services
		/// </summary>
		private void RegisterInternalServices()
		{
			// Register all classes inherited from Page in whole app
			this.RegisterPages();

			// Register all ViewModels
			this.RegisterViewModels();
			
			// Lazy service
			this.serviceCollection.AddScoped(typeof(Lazy<>), typeof(LazyService<>));

			this.serviceCollection.AddSingleton<ViewModelFactory>();
			this.serviceCollection.AddSingleton<PageFactory>();
			this.serviceCollection.AddSingleton<PageManager>();
		}

		/// <summary>
		/// Register pages from application
		/// </summary>
		private void RegisterPages()
		{
			var pages = TypeFinder.GetSubclassesOf(typeof(Page<>));

			foreach (var page in pages)
			{
				this.serviceCollection.AddTransient(page);
			}
		}

		/// <summary>
		/// Register ViewModels from application
		/// </summary>
		private void RegisterViewModels()
		{
			var viewModels = TypeFinder.GetSubclassesOf(typeof(IViewModel));

			foreach (var page in viewModels)
			{
				this.serviceCollection.AddTransient(page);
			}
		}

		#endregion
	}
}