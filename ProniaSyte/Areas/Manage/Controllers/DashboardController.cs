using Microsoft.AspNetCore.Mvc;

namespace ProniaSyte.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}
