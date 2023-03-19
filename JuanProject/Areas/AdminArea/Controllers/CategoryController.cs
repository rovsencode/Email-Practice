using JuanProject.DAL;
using JuanProject.Models;
using JuanProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JuanProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
           List<Category> categories = _appDbContext.Categories.ToList();
            List<CategoryVM> categoryList = new();
            foreach (var category in categories) {
                CategoryVM newcategory = new()
                {
                   Name = category.Name,
                   Id=category.Id,

                };
                categoryList.Add(newcategory);
            }
            
            return View(categoryList);
        }

        public ActionResult Create() {
         
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateCategoryVM category)
        {
            if (!ModelState.IsValid) return View();
            bool exist = await _appDbContext.Categories.AnyAsync(c=>c.Name.ToLower()==category.Name.ToLower());
            if (exist)
            {
                ModelState.AddModelError("Name", "bele bir category movcuddur");
                return View();
            }
            Category newcategory = new()
            {
                Name = category.Name,
                IsDeleted = false,
            };
            _appDbContext.Categories.Add(newcategory);
            _appDbContext.SaveChanges();
            return RedirectToAction ("index","category");

        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null) return NotFound();
           Category category =await _appDbContext.Categories.FindAsync(id); 
            if (category == null) return NotFound();
            return View( new CategoryUpdateVM  {
                Name= category.Name,
                Id=category.Id,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryUpdateVM category,int id,string name) {

            bool exist = await _appDbContext.Categories.AnyAsync(c=>c.Name==category.Name && c.Id==category.Id);
            if (exist)
            {
                ModelState.AddModelError("Name", "Bele bir category movcuddur");
                return View();
            }
           Category existCategory = await _appDbContext.Categories.FindAsync(id);
           existCategory.Name=category.Name;
            _appDbContext.SaveChanges();


            return RedirectToAction("index","category");
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)return NotFound();
            Category category= await _appDbContext.Categories.FindAsync(id);
            return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return NotFound();
            Category category = await _appDbContext.Categories.FindAsync(id);
            _appDbContext.Categories.Remove(category);
            _appDbContext.SaveChanges();
            return RedirectToAction("index", "category");
        }
    }
}
