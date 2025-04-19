using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Implementation
{
	public class CityRepository : ICityRepository
	{
		private readonly AppDbContext _context;

		public CityRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<City>> GetAll()
		{
			var cities = await _context.Cities.ToListAsync();
			return cities;

		}

		public async Task<IEnumerable<CityViewModel>> GetAllCityViewModels()
		{
			var cities = await _context.Cities.ToListAsync();

			var cityViewModels = new List<CityViewModel>();

			foreach (var city in cities)
			{
				var province = await _context.Provinces.FindAsync(city.ProvinceId);

				var cityViewModel = new CityViewModel
				{
					Id = city.Id,
					CityName = city.CityName,
					ProvinceName = province != null ? province.ProvinceName : string.Empty,
				};

				cityViewModels.Add(cityViewModel);
			}

			return cityViewModels;
		}

		public async Task<IEnumerable<Province>> GetAllProvince()
		{
			return await _context.Provinces.ToListAsync();
		}

		public async Task<City> GetCityById(int id)
		{
			return await _context.Cities.FindAsync(id);
		}

		public async  Task<City> GetCityByProvince(int provinceId)
		{
			return await _context.Cities.FindAsync(provinceId);
		}

		public async Task Add(City model)
		{
			await _context.Cities.AddAsync(model);
			await _context.SaveChangesAsync();
		}

		public async Task Update(City model)
		{
			var City = await _context.Cities.FindAsync(model.Id);
			if (model.Id != null)
			{
				model.Id = model.Id;
				model.CityName = model.CityName;
				model.ProvinceId = model.ProvinceId;
				_context.Update(City);
				await _context.SaveChangesAsync();
			}
		}

		public Task Delete(int id)
		{
			throw new NotImplementedException();
		}

	}
}
