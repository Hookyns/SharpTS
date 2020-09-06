using System;
using System.Collections.Generic;
using Chromely.Core;
using Chromely.Core.Host;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.Core;
using SharpTS.DI;
using SharpTS.Message;
using SharpTS.Component;
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
                window.StartWithDevTools();
            }
        }

        /// <summary>
        /// Create and setup message broker
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal void PrepareMessageBroker(ServiceProvider serviceProvider)
        {
            this.messageBroker = serviceProvider.GetRequiredService<MessageBroker>();
            this.messageBroker.On<MessageEventArgs>(MessageType.DOMContentLoaded, async (arg) =>
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
            // Register all classes inherited from Component in whole app
            this.RegisterComponents(serviceCollection);

            // Register all ViewModels
            this.RegisterViewModels(serviceCollection);

            // Lazy service
            serviceCollection.AddScoped(typeof(Lazy<>), typeof(LazyService<>));

            serviceCollection.AddSingleton<IChromelyMessageRouter, InteropMessageHandler>();
            serviceCollection.AddSingleton<MessageBroker>();

            serviceCollection.AddSingleton<ViewModelFactory>();
            serviceCollection.AddSingleton<ComponentFactory>();
            serviceCollection.AddSingleton<ComponentManager>();
            serviceCollection.AddSingleton<Window>();
        }

        /// <summary>
        /// Register components from application
        /// </summary>
        internal void RegisterComponents(ServiceCollection serviceCollection)
        {
            var components = TypeFinder.GetSubclassesOf(typeof(Component<>));

            foreach (Type component in components)
            {
                serviceCollection.AddTransient(component);
            }
        }

        /// <summary>
        /// Register ViewModels from application
        /// </summary>
        internal void RegisterViewModels(ServiceCollection serviceCollection)
        {
            ICollection<Type> viewModels = TypeFinder.GetSubclassesOf(typeof(IViewModel));

            foreach (Type component in viewModels)
            {
                serviceCollection.AddTransient(component);
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