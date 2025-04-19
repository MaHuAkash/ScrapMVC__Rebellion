using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebCWMS.Models;
using WebCWMS.Repository.Interface;
using WebCWMS.ViewModel;

namespace WebCWMS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAll();
            return View(categories);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {

            if (id == 0)
            {
                return View(new Category());
            }
            else
            {
                Category category = await _categoryRepository.GetCategoryById(id);
                if (category != null)
                {
                    return View(category);
                }
                TempData["errorMessage"] = $"category details not found with Id! : {id}";
                RedirectToAction(nameof(Index));
            }
            return View();
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(Category model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (model.Id == 0)
                    {
                        await _categoryRepository.Add(model);
                        TempData["SuccessMessage"] = "Category created successfully!";
                        return RedirectToAction(nameof(Index));
                    }

                    else
                    {
                        var existingCategory = await _categoryRepository.GetCategoryById(model.Id);
                        existingCategory.CategoryName = model.CategoryName;
                        await _categoryRepository.Update(model);
                        TempData["SuccessMessage"] = "Category Updated successfully!";

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

    }
}
