using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
	public interface ICityRepository
	{
		Task<IEnumerable<City>> GetAll();
		Task<City> GetCityById(int id);
		Task<IEnumerable<Province>> GetAllProvince();
		Task<IEnumerable<CityViewModel>> GetAllCityViewModels();
		Task<City> GetCityByProvince(int provinceId);
		Task Add(City model);
		Task Update(City model);
		Task Delete(int id);
	}
}
