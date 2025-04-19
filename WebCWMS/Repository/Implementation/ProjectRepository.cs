using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Repository.Implementation
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

		public async Task<int> GetTotalProjectsAsync()
		{
			return await _context.Projects.CountAsync();
		}

		public async Task<IEnumerable<Project>> GetAll()
        {
            var projects = await _context.Projects.ToListAsync();

            return projects;
        }
		public async Task<IEnumerable<City>> GetCitiesByProvince(int provinceId)
		{
			return await _context.Cities.Where(city => city.ProvinceId == provinceId).ToListAsync();
		}

		public async Task<Project> GetProjectById(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<IEnumerable<Province>> GetAllProvince()
        {
            return await _context.Provinces.ToListAsync();
        }


        public async Task<IEnumerable<City>> GetAllCity()
        {
            return await _context.Cities.ToListAsync();
        }
		public async Task<IEnumerable<Category>> GetAllCategory()
		{
			return await _context.Categories.ToListAsync();
		}


		public async Task<IEnumerable<Avenue>> GetAllAvenue()
        {
            return await _context.Avenues.ToListAsync();
        }



        public async Task<IEnumerable<Avenue>> GetAvenuesByProvinceAndCity(int provinceId, int? cityId)
        {
            var query = _context.Avenues.Where(avenue => avenue.ProvinceId == provinceId);

            if (cityId != null)
            {
                query = query.Where(avenue => avenue.CityId == cityId);
            }

            return await query.ToListAsync();
        }


		public async Task<IEnumerable<ProjectViewModel>> GetAllProjectViewModels()
        {
            var projects = await _context.Projects.ToListAsync();


            var projectViewModels = new List<ProjectViewModel>();

            foreach (var project in projects)
            {
                var category = await _context.Categories.FindAsync(project.CategoryId);
                var province = await _context.Provinces.FindAsync(project.ProvinceId);
                var city = await _context.Cities.FindAsync(project.CityId);
                var avenue = await _context.Avenues.FindAsync(project.AvenueId);


                var projectViewModel = new ProjectViewModel
                {
                    Id = project.Id,
                    Client = project.Client,
                    ProvinceName = province != null ? province.ProvinceName : string.Empty,
                    CategoryName= category !=null? category. CategoryName : string.Empty,
                    CityName = city != null ? city.CityName : string.Empty,
                    AvenueName = avenue != null ? avenue.AvenueName : string.Empty,
                    CreatedAt = project.CreatedAt,
                    ImageFileNames = project.ImageFileNames,
                    ProjectName = project.ProjectName,
                    Description =project.Description,


                };

                projectViewModels.Add(projectViewModel);
            }

            return projectViewModels;
        }

        public async Task Add(Project model)
        {
            await _context.Projects.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<Project> Details(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return null;
            }

            return project;
        }

		public async Task Update(Project model)
		{
			var project = await _context.Projects.FindAsync(model.Id);
			if (project != null)
			{
				// Update project properties
				project.CategoryId = model.CategoryId;
				project.ProvinceId = model.ProvinceId;
				project.CityId = model.CityId;
				project.AvenueId = model.AvenueId;
				project.CreatedAt = model.CreatedAt;
				project.Client = model.Client;
				project.ProjectName = model.ProjectName;
                project.Description= model.Description;
				project.Status = model.Status;

				// Handle image files if they are uploaded
				if (model.ImageFiles != null && model.ImageFiles.Count > 0)
				{
					string wwwRootPath = _environment.WebRootPath;
					var imageFileNames = new List<string>();

					foreach (var file in model.ImageFiles)
					{
						if (file.Length > 0)
						{
							string fileName = Path.GetFileNameWithoutExtension(file.FileName);
							string extension = Path.GetExtension(file.FileName);
							string uniqueFileName = Guid.NewGuid().ToString() + extension;
							imageFileNames.Add(uniqueFileName);
							string path = Path.Combine(wwwRootPath, "Image", uniqueFileName);

							using (var fileStream = new FileStream(path, FileMode.Create))
							{
								await file.CopyToAsync(fileStream);
							}
						}
					}

					// Update the image file names
					project.ImageFileNames = string.Join(",", imageFileNames);
				}

				// Save the updated project
				_context.Update(project);
				await _context.SaveChangesAsync();
			}
			else
			{
				throw new InvalidOperationException($"Project with ID {model.Id} not found.");
			}
		}

	}
}
