using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebCWMS.Models;
using WebCWMS.Repository.Implementation;
using WebCWMS.Repository.Interface;
using X.PagedList;

namespace WebCWMS.Controllers
{
	public class CityController : Controller
	{
		private readonly ICityRepository _cityRepository;
        private readonly AppDbContext _appDbcontext;

        public CityController(ICityRepository cityRepository, AppDbContext appDbContext)
		{
			_cityRepository = cityRepository;
			_appDbcontext = appDbContext;	

		}

        [Authorize]
        public async Task<IActionResult> Index(int? page) 
		{
			int pageNumber = page ?? 1; 
			int pageSize = 7; 

			var cityViewModels = await _cityRepository.GetAllCityViewModels();

			var pagedCityViewModels = cityViewModels.ToPagedList(pageNumber, pageSize);

			ViewBag.CityViewModel = pagedCityViewModels;

			return View(pagedCityViewModels);
		}

        [Authorize]
        [HttpGet]
		public async Task<IActionResult> CreateOrEdit(int id = 0)
		{
			var provinces = await _cityRepository.GetAllProvince();

			if (id == 0)
			{
				ViewBag.Provinces = new SelectList(provinces, "Id", "ProvinceName"); 
			}
			else
			{
				ViewBag.Provinces = new SelectList(provinces, "Id", "ProvinceName"); 
				City city = await _cityRepository.GetCityById(id);
				if (city != null)
				{
					return View(city);
				}
				TempData["errorMessage"] = $"City details not found with Id! : {id}";
				RedirectToAction(nameof(Index));
			}
			return View();
		}

        [Authorize]
        [HttpPost]
		public async Task<IActionResult> CreateOrEdit(City model)
		{
			try
			{
				if (ModelState.IsValid)
				{

					if (model.Id == 0)
					{
						await _cityRepository.Add(model);
						TempData["SuccessMessage"] = "City created successfully!";
						return RedirectToAction(nameof(Index));
					}

					else
					{
						var existingCity = await _cityRepository.GetCityById(model.Id);
						existingCity.CityName = model.CityName;
						existingCity.ProvinceId = model.ProvinceId;
						await _cityRepository.Update(model);
						TempData["SuccessMessage"] = "City Updated successfully!";

					}
					return RedirectToAction(nameof(Index));


				}
				else
				{
					TempData["errorMessage"] = "model state is invalid";
					return View();
				}

			}
			catch (Exception ex)
			{

				TempData["errorMessage"] = ex.Message;
				return View();

			}
		}

		//[HttpGet]
		//public async Task<IActionResult> Delete(int id)
		//{
		//	var cities = await _cityRepository.GetCityById(id);
		//	if (cities == null)
		//	{
		//		TempData["ErrorMessage"] = "Blog not found.";
		//		return RedirectToAction(nameof(Index));
		//	}

		//	return View(cities);
		//}

		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> DeleteConfirmed(int id)
		//{
		//	try
		//	{
		//		await _cityRepository.Delete(id);
		//		TempData["SuccessMessage"] = "City deleted successfully!";
		//		return RedirectToAction(nameof(Index));
		//	}
		//	catch (Exception ex)
		//	{
		//		TempData["ErrorMessage"] = "Error deleting the City: " + ex.Message;
		//		return RedirectToAction(nameof(Index));
		//	}
		//}

	}
}
