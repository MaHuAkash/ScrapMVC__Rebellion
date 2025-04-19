using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetCategoryById(int id);
        Task Add(Category model);
        Task Update(Category model);
        Task Delete(int id);
		Task<int> GetTotalCategoryAsync();


	}
}
