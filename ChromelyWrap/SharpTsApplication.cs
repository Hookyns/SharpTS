using System;
using Chromely.Core;
using Chromely.Core.Host;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.Core;
using SharpTS.DI;
using SharpTS.Message;
using SharpTS.Page;
using SharpTS.Reflection;
using SharpTS.ViewModel;

namespace SharpTS.ChromelyWrap
{
    /// <summary>
    /// Base SharpTs application
    /// </summary>
    public class SharpTsApplication
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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public SharpTsApplication(ApplicationBuilder applicationBuilder)
        {
            this.applicationBuilder = applicationBuilder;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void Configure(ServiceProvider serviceProvider)
        {
            AppWindow window = serviceProvider.GetRequiredService<IChromelyWindow>() as AppWindow;

            if (window == null)
            {
                throw new InvalidOperationException("Created window is not SharpTS AppWindow!");
            }

            if (this.applicationBuilder.StartWithDevToolsOpened)
            {
                window.ShowDevTools();
            }
        }

        /// <summary>
        /// Create and setup message broker
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal void PrepareMessageBroker(ServiceProvider serviceProvider)
        {
            this.messageBroker = serviceProvider.GetRequiredService<MessageBroker>();
            this.messageBroker.On(MessageType.DOMContentLoaded, async (arg) =>
            {
                await this.applicationBuilder.Application.Loaded();
                arg.Success(null);
            });
        }

        /// <summary>
        /// Register internal services
        /// </summary>
        internal void RegisterInternalServices(ServiceCollection serviceCollection)
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
            serviceCollection.AddSingleton<Window>();
        }

        /// <summary>
        /// Register pages from application
        /// </summary>
        internal void RegisterPages(ServiceCollection serviceCollection)
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
        internal void RegisterViewModels(ServiceCollection serviceCollection)
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
        internal void ConfigureApplication(ServiceProvider serviceProvider)
        {
            // Create Window
            Window window = serviceProvider.GetRequiredService<Window>();

            // Configure Application
            this.applicationBuilder.Application.SetWindow(window);
            this.applicationBuilder.Application.Configure(this.applicationBuilder);
        }

        /// <summary>
        /// Call Application Initialize hook
        /// </summary>
        internal void InitializeApplication()
        {
            this.applicationBuilder.Application.Initialize();
        }

        #endregion
    }
}