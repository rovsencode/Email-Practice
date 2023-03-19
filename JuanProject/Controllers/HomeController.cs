
using JuanProject.DAL;
using JuanProject.Models;
using JuanProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JuanProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders.Where(m => !m.IsDeleted).ToListAsync();
            List<Product> products = await _context.Products
                .Where(m=>!m.IsDeleted)
                .Include(m=>m.ProductImages)
                .Take(5)
                .ToListAsync();
            HomeVM homeVM = new HomeVM()
            { 
                Sliders = sliders,
                Products= products,
            };

            return View(homeVM);    
          
        }

    }
}