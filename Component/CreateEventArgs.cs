using Newtonsoft.Json.Linq;

namespace SharpTS.Component
{
	/// <summary>
	/// Navigation event args
	/// </summary>
	public class CreateEventArgs
	{
		/// <summary>
		/// Navigation parameters
		/// </summary>
		public JObject Parameters { get; }

		/// <summary>
		/// Identifier of previous component
		/// </summary>
		public ComponentIdentifier PreviousComponent { get; }

		/// <summary>
		/// Parent component identifier
		/// </summary>
		public ComponentIdentifier ParentComponent { get; }

		/// <summary>
		/// If Component is displayed as partial
		/// </summary>
		public bool IsPartial => this.ParentComponent != null;
		
		/// <summary>
		/// Default action is prevented
		/// </summary>
		public bool DefaultPrevented { get; private set; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="previousComponent"></param>
		/// <param name="parentComponent"></param>
		public CreateEventArgs(JObject parameters, ComponentIdentifier previousComponent, ComponentIdentifier parentComponent)
		{
			this.Parameters = parameters;
			this.PreviousComponent = previousComponent;
			this.ParentComponent = parentComponent;
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