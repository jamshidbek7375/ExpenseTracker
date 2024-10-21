using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Application.ViewModels.Category;
using ExpenseTracker.Stores.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryStore _store;

    public CategoriesController(ICategoryStore store)
    {
        _store = store;
    }

    public IActionResult Index([FromQuery] GetCategoriesRequest request)
    {
        var categories = _store.GetAll(request);

        ViewBag.Search = request.Search;

        return View(categories);
    }

    public IActionResult Details([FromRoute] CategoryRequest request)
    {
        var category = _store.GetById(request);

        return Json(category);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([FromForm] CreateCategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();

            return Json(new { success = false, errors });
        }

        _store.Create(request);

        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([FromRoute] int id, [FromForm] UpdateCategoryRequest request)
    {
        if (id != request.Id)
        {
            return Json(new { success = false, message = "Route id does not match with body id." });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            return Json(new { success = false, errors });
        }

        _store.Update(request);

        return Json(new { success = true });
    }

    [HttpDelete, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed([FromRoute] CategoryRequest request)
    {
        _store.Delete(request);

        return Json(new { success = true });
    }

    /// <summary>
    /// Filters categories
    /// </summary>
    /// <param name="search"></param>
    /// <returns>List of filtered categories</returns>
    [Route("getCategories")]
    public ActionResult<CategoryViewModel> GetCategories([FromQuery] GetCategoriesRequest request)
    {
        var categories = _store.GetAll(request);

        return Ok(categories);
    }
}