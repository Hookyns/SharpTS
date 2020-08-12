using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using SharpTS.Annotations;
using SharpTS.ViewModel;
using SharpTS.Component;

namespace SharpTS.ViewModel
{
	public class TestViewModel : BaseViewModel, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		/// <summary>
		/// Name
		/// </summary>
		public virtual string Name { get; set; }

		public TestViewModel(ComponentFactory scope)
		{
			this.PropertyChanged += (sender, args) =>
			{
				Console.WriteLine("Changed");
			};
		}
	}
}