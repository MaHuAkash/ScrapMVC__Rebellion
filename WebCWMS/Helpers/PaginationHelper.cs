	using X.PagedList; // Make sure to include the necessary namespace for IPagedList.

namespace WebCWMS.Helpers
{
	public class PaginationHelper
	{
		public static IPagedList<T> GetPagedList<T>(IEnumerable<T> source, int pageNumber, int pageSize)
		{
			return new PagedList<T>(source, pageNumber, pageSize);
		}
	}
}
