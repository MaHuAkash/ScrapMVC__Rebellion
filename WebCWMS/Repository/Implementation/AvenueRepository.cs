using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Implementation
{
	public class AvenueRepository : IAvenueRepository
	{
		private readonly AppDbContext _context;

		public AvenueRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Avenue>> GetAll()
		{
			var avenues = await _context.Avenues.ToListAsync();
			return avenues;
		}

		public async Task<City> GetCityByProvince(int provinceId)
		{
			return await _context.Cities.FindAsync(provinceId);
		}


		public async Task<IEnumerable<AvenueViewModel>> GetAllAvenueViewModels()
		{
			var avenues = await _context.Avenues.ToListAsync();

			var avenueViewModels = new List<AvenueViewModel>();

			foreach (var avenue in avenues)
			{
				var province = await _context.Provinces.FindAsync(avenue.ProvinceId);
				var city = await _context.Cities.FindAsync(avenue.CityId);

				var avenueViewModel = new AvenueViewModel
				{
					Id = avenue.Id,
					AvenueName = avenue.AvenueName,
					ProvinceName = province != null ? province.ProvinceName : string.Empty,
					CityName = city != null ? city.CityName : string.Empty,

				};

				avenueViewModels.Add(avenueViewModel);
			}

			return avenueViewModels;
		}

		public async Task<IEnumerable<City>> GetAllCity()
		{
			return await _context.Cities.ToListAsync();
		}

		public async Task<IEnumerable<Province>> GetAllProvince()
		{
			return await _context.Provinces.ToListAsync();
		}

		public async Task<Avenue> GetAvenueById(int id)
		{
			return await _context.Avenues.FindAsync(id);
		}

		public async Task Add(Avenue model)
		{
			await _context.Avenues.AddAsync(model);
			await _context.SaveChangesAsync();
		}

		public async Task Update(Avenue model)
		{
			var Avenue = await _context.Avenues.FindAsync(model.Id);
			if (model.Id != null)
			{
				model.Id = model.Id;
				model.AvenueName = model.AvenueName;
				model.ProvinceId = model.ProvinceId;
				model.CityId= model.CityId;
				_context.Update(Avenue);
				await _context.SaveChangesAsync();
			}

		}

		public async Task Delete(int id)
		{
			var avenue = await _context.Avenues.FindAsync(id);
			if (avenue != null)
			{
				_context.Avenues.Remove(avenue);
				await _context.SaveChangesAsync();
			}
		}

	}
}
