using System;
using Xilium.CefGlue;

namespace SharpTS.ChromelyWrap
{
	/// <summary>
	/// Generic CefTask implementation
	/// </summary>
	internal class GenericCefTask : CefTask
	{
		/// <summary>
		/// Action to run
		/// </summary>
		private readonly Action action;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="action"></param>
		public GenericCefTask(Action action)
		{
			this.action = action;
		}

		/// <summary>
		/// Execute action
		/// </summary>
		protected override void Execute()
		{
			this.action.Invoke();
		}
	}
}