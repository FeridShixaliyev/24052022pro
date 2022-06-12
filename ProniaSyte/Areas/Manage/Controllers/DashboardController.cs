using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProniaSyte.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles ="Admin,Moderator")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}
