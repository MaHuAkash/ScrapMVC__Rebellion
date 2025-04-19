using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
    public interface IProvinceRepository
    {
        Task<IEnumerable<Province>> GetAll();
        Task<Province> GetProvinceById(int id);
        Task Add(Province model);
        Task Update(Province model);
		Task Delete(int id);


	}
}
