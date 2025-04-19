using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
	public interface IAvenueRepository
	{
		Task<IEnumerable<Avenue>> GetAll();
		Task<Avenue> GetAvenueById(int id);
		Task<IEnumerable<Province>> GetAllProvince();
		Task<IEnumerable<City>> GetAllCity();
		Task<IEnumerable<AvenueViewModel>> GetAllAvenueViewModels();
		Task Add(Avenue model);
		Task Update(Avenue model);
		Task Delete(int id);
		Task<City> GetCityByProvince(int provinceId);
	}
}
