using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCWMS.Models;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Interface
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAll();
        Task<Project> GetProjectById(int id);
        Task Add(Project project);
        Task Update(Project project);
        Task Delete(int id);
        Task<Project> Details(int id);
        Task<IEnumerable<Province>> GetAllProvince();
        Task<IEnumerable<City>> GetAllCity();
        Task<IEnumerable<Avenue>> GetAllAvenue();
		Task<IEnumerable<Category>> GetAllCategory();

		Task<IEnumerable<ProjectViewModel>> GetAllProjectViewModels();
        Task<IEnumerable<City>> GetCitiesByProvince(int provinceId);
        Task<IEnumerable<Avenue>> GetAvenuesByProvinceAndCity(int provinceId, int? cityId);
		Task<int> GetTotalProjectsAsync();


	}
}
