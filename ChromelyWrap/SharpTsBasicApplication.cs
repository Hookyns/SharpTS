using System;
using Chromely;
using Chromely.Browser;
using Chromely.Core.Host;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.Core;
using SharpTS.DI;
using SharpTS.Message;
using SharpTS.Page;
using SharpTS.Reflection;
using SharpTS.ViewModel;
using Xilium.CefGlue;
using Window = SharpTS.Core.Window;

namespace SharpTS.ChromelyWrap
{
	public sealed class SharpTsBasicApplication : ChromelyBasicApp
	{
		/// <summary>
		/// Application
		/// </summary>
		private readonly Application application;


		/// <summary>
		/// Interop message broker
		/// </summary>
		private MessageBroker messageBroker;

		public SharpTsBasicApplication(Application application)
		{
			this.application = application;
		}

		public override void Initialize(ServiceProvider serviceProvider)
		{
			base.Initialize(serviceProvider);

			AppWindow window = serviceProvider.GetRequiredService<IChromelyWindow>() as AppWindow;
			bool initiated = false;
			
			void FrameLoadEnd(object sender, FrameLoadEndEventArgs args)
			{
				if (initiated)
				{
					return;
				}
				
				initiated = true;
				window.FrameLoadEnd -= FrameLoadEnd;
				
				CefFrame frame = window.Browser.GetMainFrame();

				this.messageBroker = new MessageBroker(frame);
				this.messageBroker.On(MessageType.DOMContentLoaded, async (arg) =>
				{
					await this.application.OnLaunched();
					arg.Success(null);
				});

				this.InitializeApplication();
			};

			window!.FrameLoadEnd += FrameLoadEnd;
		}

		public override void ConfigureServices(ServiceCollection serviceCollection)
		{
			base.ConfigureServices(serviceCollection);
			this.RegisterInternalServices(serviceCollection);
		}


		/// <summary>
		/// Register internal services
		/// </summary>
		private void RegisterInternalServices(ServiceCollection serviceCollection)
		{
			// Register all classes inherited from Page in whole app
			this.RegisterPages(serviceCollection);

			// Register all ViewModels
			this.RegisterViewModels(serviceCollection);
		
			
			// Lazy service
			serviceCollection.AddScoped(typeof(Lazy<>), typeof(LazyService<>));

			serviceCollection.AddSingleton<ViewModelFactory>();
			serviceCollection.AddSingleton<PageFactory>();
			serviceCollection.AddSingleton<PageManager>();
		}

		/// <summary>
		/// Register pages from application
		/// </summary>
		private void RegisterPages(ServiceCollection serviceCollection)
		{
			var pages = TypeFinder.GetSubclassesOf(typeof(Page<>));

			foreach (var page in pages)
			{
				serviceCollection.AddTransient(page);
			}
		}

		/// <summary>
		/// Register ViewModels from application
		/// </summary>
		private void RegisterViewModels(ServiceCollection serviceCollection)
		{
			var viewModels = TypeFinder.GetSubclassesOf(typeof(IViewModel));

			foreach (var page in viewModels)
			{
				serviceCollection.AddTransient(page);
			}
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
	}
}