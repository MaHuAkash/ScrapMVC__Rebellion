using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Implementation
{
	public class BlogRepository : IBlogRepository
	{

		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;


		public async Task<int> GetTotalBlogAsync()
		{
			return await _context.Blogs.CountAsync();
		}
		public BlogRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Blog>> GetAll()
		{
			var blogs = await _context.Blogs.ToListAsync();

			return blogs;
		}

		public async Task<Blog> GetBlogById(int id)
		{
			return await _context.Blogs.FindAsync(id);
		}

		public async Task<IEnumerable<Province>> GetAllProvince()
		{
			return await _context.Provinces.ToListAsync();
		}






		public async Task<IEnumerable<BlogViewModel>> GetAllBlogViewModels()
		{
			var blogs = await _context.Blogs.ToListAsync();

			
			var blogViewModels = new List<BlogViewModel>();

			foreach (var blog in blogs)
			{

				var blogViewModel = new BlogViewModel
				{
					Id = blog.Id,
					Date = blog.Date,
					Title = blog.Title,
					Description = blog.Description,
					ImageName = blog.ImageName,


				};

				blogViewModels.Add(blogViewModel);
			}

			return blogViewModels;
		}

		public async Task Add(Blog model)
		{
			await _context.Blogs.AddAsync(model);
			await _context.SaveChangesAsync();
		}
		public async Task Update(Blog model)
		{
			var Blog = await _context.Blogs.FindAsync(model.Id);
			if (model.Id != null)
			{
				model.Id = model.Id;
				model.Date = model.Date;
				model.Title = model.Title;
				model.Description = model.Description;
				if (model.ImageFile != null)
				{
					string wwwRootPath = _webHostEnvironment.WebRootPath;
					string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
					string extension = Path.GetExtension(model.ImageFile.FileName);
					string uniqueFileName = Guid.NewGuid().ToString() + extension;
					Blog.ImageName = uniqueFileName;
					string path = Path.Combine(wwwRootPath, "Image", uniqueFileName);
					using (var filestream = new FileStream(path, FileMode.Create))
					{
						await model.ImageFile.CopyToAsync(filestream);
					}
				}
				_context.Update(Blog);
				await _context.SaveChangesAsync();
			}

		}
		public async Task Delete(int id)
		{
			var blog = await _context.Blogs.FindAsync(id);
			if (blog != null)
			{
				_context.Blogs.Remove(blog);
				await _context.SaveChangesAsync();
			}
		}


		public async Task<Blog> Details(int id)
		{
			var blog = await _context.Blogs.FindAsync(id);

			if (blog == null)
			{
				return null;
			}

			return blog;
		}



	}

}

