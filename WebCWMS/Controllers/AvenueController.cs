using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebCWMS.Models;
using WebCWMS.Repository.Implementation;
using WebCWMS.Repository.Interface;

namespace WebCWMS.Controllers
{
	public class AvenueController : Controller
	{
		private readonly IAvenueRepository _avenueRepository; 
		private readonly IProjectRepository _projectRepository;
		private readonly AppDbContext _appDbContext;

		public AvenueController(IAvenueRepository avenueRepository, IProjectRepository projectRepository ,AppDbContext appDbContext, IBlogRepository blogRepository)
		{
			_avenueRepository = avenueRepository;
			_appDbContext = appDbContext;
			_projectRepository = projectRepository;
		}

        [Authorize(Roles = "Superadmin")]
        public async Task<IActionResult> Index()
		{
			var avenueViewModels = await _avenueRepository.GetAllAvenueViewModels();
			return View(avenueViewModels);
		}

        [Authorize(Roles = "Superadmin")]
        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
		{
			var provinces = await _avenueRepository.GetAllProvince();
			var city = await _avenueRepository.GetAllCity();

			if (id == 0)
			{
				ViewBag.Provinces = new SelectList(provinces, "Id", "ProvinceName"); // Assign the locations to ViewBag
				ViewBag.Cities = new SelectList(city, "Id", "CityName"); // Assign the locations to ViewBag
				return View(new Avenue());
			}
			else
			{
				ViewBag.Provinces = new SelectList(provinces, "Id", "ProvinceName"); // Assign the locations to ViewBag
				ViewBag.Cities = new SelectList(city, "Id", "CityName"); // Assign the locations to ViewBag
				Avenue avenue = await _avenueRepository.GetAvenueById(id);
				if (avenue != null)
				{
					return View(avenue);
				}
				TempData["errorMessage"] = $"Avenue details not found with Id! : {id}";
				RedirectToAction(nameof(Index));
			}
			return View();
		}

        [Authorize(Roles = "Superadmin")]
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(Avenue model)
		{
			try
			{
				if (ModelState.IsValid)
				{

					if (model.Id == 0)
					{
						var provinces = await _avenueRepository.GetAllProvince();
						var city = await _avenueRepository.GetAllCity();
						ViewBag.Provinces = new SelectList(provinces, "Id", "ProvincesName"); // Assign the locations to ViewBag
						ViewBag.Cities = new SelectList(city, "Id", "CityName"); // Assign the locations to ViewBag
						await _avenueRepository.Add(model);
						TempData["SuccessMessage"] = "Avenue created successfully!";
					}
					else
					{
						var existingAvenue = await _avenueRepository.GetAvenueById(model.Id);
						existingAvenue.AvenueName = model.AvenueName;
						existingAvenue.ProvinceId = model.ProvinceId;
						existingAvenue.CityId = model.CityId;
						
						await _avenueRepository.Update(existingAvenue);
						TempData["SuccessMessage"] = "Avenue updated successfully!";
					}

					return RedirectToAction(nameof(Index));
				}
				else
				{
					TempData["ErrorMessage"] = "Model state is invalid";
					return View(model);
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return View(model);
			}
		}
        [Authorize(Roles = "Superadmin")]
        [HttpGet]
		public async Task<IActionResult> Delete(int id)
		{
			var avenue = await _avenueRepository.GetAvenueById(id);
			if (avenue == null)
			{
				TempData["ErrorMessage"] = "Avenue not found.";
				return RedirectToAction(nameof(Index));
			}

			return View(avenue);
		}

        [Authorize(Roles = "Superadmin")]
        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			try
			{
				await _avenueRepository.Delete(id);
				TempData["SuccessMessage"] = "Avenue deleted successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error deleting the Avenue: " + ex.Message;
				return RedirectToAction(nameof(Index));
			}
		}

        [HttpGet]
        public IActionResult GetCitiesByProvince(int provinceId)
        {
            var cities = _appDbContext.Cities.Where(c => c.ProvinceId == provinceId).ToList();
            var cityList = cities.Select(c => new
            {
                value = c.Id,
                text = c.CityName
            });

            return Json(cityList);
        }

        public async Task<IActionResult> GetCitiesAndAvenuesByProvinceAndCity(int provinceId, int? cityId)
        {
            var cities = await _projectRepository.GetCitiesByProvince(provinceId);
            var avenues = await _projectRepository.GetAvenuesByProvinceAndCity(provinceId, cityId);

            var cityItems = cities.Select(city => new SelectListItem
            {
                Value = city.Id.ToString(),
                Text = city.CityName
            });

            var avenueItems = avenues.Select(avenue => new SelectListItem
            {
                Value = avenue.Id.ToString(),
                Text = avenue.AvenueName
            });

            var result = new
            {
                cities = cityItems,
                avenues = avenueItems
            };

            return Json(result);
        }



    }
}
