using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.ViewModel;

namespace SharpTS.Component
{
	/// <summary>
	/// Base class for Component
	/// </summary>
	/// <typeparam name="TViewModel"></typeparam>
	public abstract class Component<TViewModel> : IInternalComponent<TViewModel>
		where TViewModel : class, IViewModel
	{
		#region Fields

		/// <summary>
		/// Component identifier
		/// </summary>
		private ComponentIdentifier identifier;

		#endregion

		#region Properties

		/// <summary>
		/// Component View-Model
		/// </summary>
		protected internal TViewModel ViewModel { get; internal set; }

		/// <summary>
		/// Component identifier
		/// </summary>
		public ComponentIdentifier Identifier => this.identifier ??= ComponentIdentifier.GetIdentifier(this);

		/// <summary>
		/// DI Service scope
		/// </summary>
		internal IServiceScope ServiceScope { get; set; }

		/// <summary>
		/// ViewModel factory
		/// </summary>
		internal ViewModelFactory ViewModelFactory { get; set; }

		/// <summary>
		/// DI Service scope
		/// </summary>
		IServiceScope IInternalComponent<TViewModel>.ServiceScope
		{
			get => this.ServiceScope;
			set => this.ServiceScope = value;
		}

		/// <summary>
		/// ViewModel factory
		/// </summary>
		ViewModelFactory IInternalComponent<TViewModel>.ViewModelFactory
		{
			get => this.ViewModelFactory;
			set => this.ViewModelFactory = value;
		}

		#endregion

		#region Ctors

		/// <summary>
		/// Dispose
		/// </summary>
		public virtual void Dispose()
		{
			// Dispose service scope
			this.ServiceScope?.Dispose();
			
			// Check if ViewModel is IDisposable; Dispose if it is
			if (this.ViewModel is IDisposable vmDisposable)
			{
				vmDisposable.Dispose();
			}
		}

		#endregion

		#region Methods

		/// <inheritdoc cref="IInternalComponent{TViewModel}.Created"/>
		protected virtual Task Created(CreateEventArgs args)
		{
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="IInternalComponent{TViewModel}.Destroyed"/>
		protected virtual Task Destroyed()
		{
			return Task.CompletedTask;
		}
		
		/// <inheritdoc cref="IInternalComponent{TViewModel}.Activated" />
		protected virtual Task Activated()
		{
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="IInternalComponent{TViewModel}.Deactivated" />
		protected virtual Task Deactivated()
		{
			return Task.CompletedTask;
		}

		/// <inheritdoc cref="IInternalComponent{TViewModel}.CreateViewModel"/>
		protected virtual Task CreateViewModel()
		{
			this.ViewModel = this.ViewModelFactory.Create<TViewModel>(this.ServiceScope);
			return Task.CompletedTask;
		}

		#endregion

		#region Private methods

		/// <inheritdoc />
		async Task IInternalComponent<TViewModel>.Activated()
		{
			await this.Activated();
		}

		/// <inheritdoc />
		async Task IInternalComponent<TViewModel>.Deactivated()
		{
			await this.Deactivated();
		}

		/// <inheritdoc />
		async Task IInternalComponent<TViewModel>.Created(CreateEventArgs args)
		{
			await this.Created(args);
		}

		/// <inheritdoc />
		async Task IInternalComponent<TViewModel>.Destroyed()
		{
			await this.Destroyed();
		}

		/// <inheritdoc />
		async Task IInternalComponent<TViewModel>.CreateViewModel()
		{
			await this.CreateViewModel();
		}

		#endregion
	}
}