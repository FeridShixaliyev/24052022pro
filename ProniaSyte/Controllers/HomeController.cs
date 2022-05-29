using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProniaSyte.DAL;
using ProniaSyte.Models;
using ProniaSyte.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProniaSyte.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _sql;
        public HomeController(AppDbContext sql)
        {
            _sql = sql;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Sliders = await _sql.Sliders.ToListAsync(),
                Categories=await _sql.Categories.ToListAsync(),
                Products=await _sql.Products.Include(p=>p.Images).Include(p=>p.Category).Take(8).ToListAsync()
        };
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
