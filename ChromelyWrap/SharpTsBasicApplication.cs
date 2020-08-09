using System;
using Chromely;
using Chromely.Core;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.Core;
using SharpTS.DI;
using SharpTS.Message;
using SharpTS.Page;
using SharpTS.Reflection;
using SharpTS.ViewModel;
using Window = SharpTS.Core.Window;

namespace SharpTS.ChromelyWrap
{
	public sealed class SharpTsBasicApplication : ChromelyBasicApp
	{
		#region Fields

		/// <summary>
		/// Application
		/// </summary>
		private readonly ApplicationBuilder applicationBuilder;


		/// <summary>
		/// Interop message broker
		/// </summary>
		private MessageBroker messageBroker;

		#endregion

		#region Ctors

		public SharpTsBasicApplication(ApplicationBuilder applicationBuilder)
		{
			this.applicationBuilder = applicationBuilder;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Configure DI services
		/// </summary>
		/// <param name="serviceCollection"></param>
		public override void ConfigureServices(ServiceCollection serviceCollection)
		{
			base.ConfigureServices(serviceCollection);
			this.RegisterInternalServices(serviceCollection);
		}

		/// <summary>
		/// Initialize application after setup
		/// </summary>
		/// <param name="serviceProvider"></param>
		public override void Initialize(ServiceProvider serviceProvider)
		{
			base.Initialize(serviceProvider);
			
			this.PrepareMessageBroker(serviceProvider);
			this.InitializeApplication();

			// AppWindow window = serviceProvider.GetRequiredService<IChromelyWindow>() as AppWindow;
			
			
			// bool initiated = false;
			//
			// // window!.BrowserCreated += () =>
			// // {
			// // 	this.PrepareMessageBroker(serviceProvider, window);
			// // 	this.InitializeApplication();
			// // };
			//
			// void OnBrowserCreated(object sender, EventArgs args)
			// {
			// 	if (initiated)
			// 	{
			// 		return;
			// 	}
			// 	
			// 	initiated = true;
			// 	window.Created -= OnBrowserCreated;
			// 	
			// 	this.PrepareMessageBroker(serviceProvider);
			// 	this.InitializeApplication();
			// }
			//
			// window!.Created += OnBrowserCreated;
			
			// void FrameLoadStart(object sender, FrameLoadStartEventArgs args)
			// {
			// 	if (initiated)
			// 	{
			// 		return;
			// 	}
			// 	
			// 	initiated = true;
			// 	window.FrameLoadStart -= FrameLoadStart;
			// 	
			// 	this.PrepareMessageBroker(serviceProvider, window);
			// 	this.InitializeApplication();
			// };
			//
			// window!.FrameLoadStart += FrameLoadStart;
			
			
			// window!.LoadingStateChanged += OnWindowOnLoadingStateChanged;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Create and setup message broker
		/// </summary>
		/// <param name="serviceProvider"></param>
		private void PrepareMessageBroker(ServiceProvider serviceProvider)
		{
			this.messageBroker = serviceProvider.GetRequiredService<MessageBroker>();
			this.messageBroker.On(MessageType.DOMContentLoaded, async (arg) =>
			{
				await this.applicationBuilder.Application.OnLaunched();
				arg.Success(null);
			});
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
			
			serviceCollection.AddSingleton<IChromelyMessageRouter, InteropMessageHandler>();
			serviceCollection.AddSingleton<MessageBroker>();

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
			this.applicationBuilder.Application.SetWindow(window);
			this.applicationBuilder.Application.Initialize();
		}

		#endregion
	}
}