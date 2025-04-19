using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;

namespace WebCWMS.Repository.Implementation
{
	public class ProvinceRepository : IProvinceRepository
	{
		private readonly AppDbContext _context;

		public ProvinceRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Province>> GetAll()
		{
			var provinces = await _context.Provinces.ToListAsync();
			return provinces;
		}


		public async Task<Province> GetProvinceById(int id)
		{
			return await _context.Provinces.FindAsync(id);
		}


		public async Task Add(Province model)
		{
			await _context.Provinces.AddAsync(model);
			await _context.SaveChangesAsync();
		}


		public async Task Update(Province model)
		{
			var provinces = await _context.Provinces.FindAsync(model.Id);
			if (provinces != null)
			{
				provinces.ProvinceName = model.ProvinceName;
				_context.Update(provinces);
				await _context.SaveChangesAsync();
			}
		}

		public async Task Delete(int id)
		{
			var provinces = await _context.Provinces.FindAsync(id);
			if (provinces != null)
			{
				_context.Provinces.Remove(provinces);
				await _context.SaveChangesAsync();
			}


		}
	}
}
