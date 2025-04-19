using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
	public interface IBlogRepository
	{
		// Existing methods
		Task<IEnumerable<Blog>> GetAll();
		Task<Blog> GetBlogById(int id);
		Task Add(Blog blog);
		Task Update(Blog blog);
		Task Delete(int id);
		Task<Blog> Details(int id);
		Task<IEnumerable<BlogViewModel>> GetAllBlogViewModels();
		Task<int> GetTotalBlogAsync();
	}
}
