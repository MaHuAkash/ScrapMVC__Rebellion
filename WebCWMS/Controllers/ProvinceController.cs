using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebCWMS.Models;
using WebCWMS.Repository.Implementation;
using WebCWMS.Repository.Interface;

namespace WebCWMS.Controllers
{
	public class ProvinceController : Controller
	{
		private readonly IProvinceRepository _provienceRepository;
		public ProvinceController(IProvinceRepository provienceRepository)
		{
			_provienceRepository = provienceRepository;

		}

		[Authorize]
		public async Task<IActionResult> Index()
		{
			var provinces = await _provienceRepository.GetAll();
			return View(provinces);
		}



		[Authorize]
        [HttpGet]
		public async Task<IActionResult> CreateOrEdit(int id = 0)
		{

			if (id == 0)
			{
				return View(new Province());
			}
			else
			{
				Province province = await _provienceRepository.GetProvinceById(id);
				if (province != null)
				{
					return View(province);
				}
				TempData["errorMessage"] = $"Province details not found with Id! : {id}";
				RedirectToAction(nameof(Index));
			}
			return View();
		}



		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateOrEdit(Province model)
		{
			try
			{
				if (ModelState.IsValid)
				{

					if (model.Id == 0)
					{
						await _provienceRepository.Add(model);
						TempData["SuccessMessage"] = "Province created successfully!";
						return RedirectToAction(nameof(Index));
					}

					else
					{
						var existingProvince = await _provienceRepository.GetProvinceById(model.Id);
						existingProvince.ProvinceName = model.ProvinceName;
						await _provienceRepository.Update(model);
						TempData["SuccessMessage"] = "Province Updated successfully!";

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
		//	var province = await _provienceRepository.GetProvinceById(id);
		//	if (province == null)
		//	{
		//		TempData["ErrorMessage"] = "Blog not found.";
		//		return RedirectToAction(nameof(Index));
		//	}

		//	return View(province);
		//}

		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> DeleteConfirmed(int id)
		//{
		//	try
		//	{
		//		await _provienceRepository.Delete(id);
		//		TempData["SuccessMessage"] = "Blog deleted successfully!";
		//		return RedirectToAction(nameof(Index));
		//	}
		//	catch (Exception ex)
		//	{
		//		TempData["ErrorMessage"] = "Error deleting the blog: " + ex.Message;
		//		return RedirectToAction(nameof(Index));
		//	}
		//}









	}
}
