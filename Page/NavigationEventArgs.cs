using Newtonsoft.Json.Linq;

namespace SharpTS.Page
{
	/// <summary>
	/// Navigation event args
	/// </summary>
	public class NavigationEventArgs
	{
		/// <summary>
		/// Navigation parameters
		/// </summary>
		public JObject Parameters { get; }

		/// <summary>
		/// Identifier of previous page
		/// </summary>
		public PageIdentifier PreviousPage { get; }

		/// <summary>
		/// Parent page identifier
		/// </summary>
		public PageIdentifier ParentPage { get; }

		/// <summary>
		/// If Page is displayed as partial
		/// </summary>
		public bool IsPartial => this.ParentPage != null;
		
		/// <summary>
		/// Default action is prevented
		/// </summary>
		public bool DefaultPrevented { get; private set; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="previousPage"></param>
		/// <param name="parentPage"></param>
		public NavigationEventArgs(JObject parameters, PageIdentifier previousPage, PageIdentifier parentPage)
		{
			this.Parameters = parameters;
			this.PreviousPage = previousPage;
			this.ParentPage = parentPage;
		}

		/// <summary>
		/// Map properties to required type
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <returns></returns>
		public TModel GetParametersAs<TModel>()
		{
			return this.Parameters.ToObject<TModel>();
		}

		/// <summary>
		/// Prevent default action
		/// </summary>
		public void PreventDefault()
		{
			this.DefaultPrevented = true;
		}
	}
}