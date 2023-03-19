using Microsoft.AspNetCore.Mvc;

namespace JuanProject.Areas.Admin.Controllers
{
    [Area("AdminArea")]
    public class Dashboard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
