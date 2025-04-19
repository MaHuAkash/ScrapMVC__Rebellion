using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;

namespace WebCWMS.Repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }


		public async Task<int> GetTotalCategoryAsync()
		{
			return await _context.Categories.CountAsync();
		}

		public async Task<IEnumerable<Category>> GetAll()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories;
        }


        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }


        public async Task Add(Category model)
        {
            await _context.Categories.AddAsync(model);
            await _context.SaveChangesAsync();
        }


        public async Task Update(Category model)
        {
            var categories = await _context.Categories.FindAsync(model.Id);
            if (categories != null)
            {
                categories.CategoryName = model.CategoryName;
                _context.Update(categories);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var categories = await _context.Categories.FindAsync(id);
            if (categories != null)
            {
                _context.Categories.Remove(categories);
                await _context.SaveChangesAsync();
            }


        }
    }
}
