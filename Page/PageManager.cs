using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SharpTS.ViewModel;

namespace SharpTS.Page
{
	public class PageManager
	{
		#region Fields

		/// <summary>
		/// Page factory
		/// </summary>
		private readonly PageFactory pageFactory;

		/// <summary>
		/// Dictionary holding existing page instances
		/// </summary>
		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<PageIdentifier, IInternalPage<IViewModel>>>
			pages = new ConcurrentDictionary<Type, ConcurrentDictionary<PageIdentifier, IInternalPage<IViewModel>>>();

		#endregion

		#region Ctors

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="pageFactory"></param>
		public PageManager(PageFactory pageFactory)
		{
			this.pageFactory = pageFactory;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Create new page instance
		/// </summary>
		/// <param name="pageIdentifier"></param>
		/// <returns></returns>
		public IGenericPage<IViewModel> CreateNew(PageIdentifier pageIdentifier)
		{
			var page = this.pageFactory.Create(pageIdentifier.PageType);
			this.StorePage(page);
			return page;
		}

		/// <summary>
		/// Get existing page instance or create new
		/// </summary>
		/// <param name="pageIdentifier"></param>
		/// <returns></returns>
		public IGenericPage<IViewModel> Get(PageIdentifier pageIdentifier)
		{
			return this.Find(pageIdentifier) ?? this.CreateNew(pageIdentifier);
		}

		/// <summary>
		/// Find existing instance
		/// </summary>
		/// <param name="pageIdentifier"></param>
		/// <returns></returns>
		public IGenericPage<IViewModel> Find(PageIdentifier pageIdentifier)
		{
			if (this.pages.TryGetValue(pageIdentifier.PageType, out var instancesDist))
			{
				if (instancesDist.TryGetValue(pageIdentifier, out var page))
				{
					return page;
				}
			}

			return null;
		}

		/// <summary>
		/// List existing page instances
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IGenericPage<IViewModel>> ListPages()
		{
			return this.pages.SelectMany(type => type.Value.Select(instance => instance.Value));
		}

		/// <summary>
		/// Destroy page - remove references and dispose
		/// </summary>
		/// <param name="pageIdentifier"></param>
		public void Destroy(PageIdentifier pageIdentifier)
		{
			if (this.pages.TryGetValue(pageIdentifier.PageType, out var instancesDist))
			{
				IInternalPage<IViewModel> page;
				if (instancesDist.TryRemove(pageIdentifier, out page))
				{
					page.Dispose();
				}
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Store new page
		/// </summary>
		/// <param name="page"></param>
		private void StorePage(IGenericPage<IViewModel> page)
		{
			if (!this.pages.TryGetValue(page.Identifier.PageType, out var instancesDict))
			{
				instancesDict = new ConcurrentDictionary<PageIdentifier, IInternalPage<IViewModel>>();
			}

			instancesDict.TryAdd(page.Identifier, (IInternalPage<IViewModel>)page);
		}

		#endregion
	}
}