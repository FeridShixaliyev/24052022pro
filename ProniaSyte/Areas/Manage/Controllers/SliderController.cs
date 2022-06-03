using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaSyte.DAL;
using ProniaSyte.Extentions;
using ProniaSyte.Helpers;
using ProniaSyte.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProniaSyte.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _sql;
        public SliderController(AppDbContext sql, IWebHostEnvironment env)
        {
            _sql = sql;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _sql.Sliders.ToListAsync();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid) return View();
            if (slider == null) return NotFound();
            if (slider.ImageFile != null)
            {
                if (!slider.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Sekil duzgun formatda deyil!");
                    return View();
                }
                if (!slider.ImageFile.IsSizeOk(5))
                {
                    ModelState.AddModelError("ImageFile", "Sekil 5 mb-dan boyuk ola bilmez!");
                }
                slider.Image = slider.ImageFile.SavaImage(_env.WebRootPath, "assets/images/slider/inner-img");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Bele bir sekil yoxdur!!");
                return View();
            }
            _sql.Sliders.Add(slider);
            _sql.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            Slider slider = _sql.Sliders.FirstOrDefault(s => s.Id == id);
            if (id == null) return NotFound();
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid) return View();
            Slider existSlider = _sql.Sliders.FirstOrDefault(s => s.Id == slider.Id);
            if (existSlider == null) return NotFound();
            if (slider.ImageFile != null)
            {
                if (!slider.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Sekilin formati duzgun deyil!");
                    return View();
                }
                if (!slider.ImageFile.IsSizeOk(5))
                {
                    ModelState.AddModelError("ImageFile", "Sekil 5 mb-dan boyuk ola bilmez!");
                    return View();
                } else
                {
                    Helper.DeleteImg(_env.WebRootPath, "assets/images/slider/inner-img", existSlider.Image);
                    existSlider.Image = slider.ImageFile.SavaImage(_env.WebRootPath, "assets/images/slider/inner-img");
                    existSlider.Title = slider.Title;
                    existSlider.DiscountPercent = slider.DiscountPercent;
                    existSlider.Description = slider.Description;
                }

            }
            else
            {
                ModelState.AddModelError("ImageFile", "Sekil secin");
                return View();
            }


            _sql.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public IActionResult Delete(int? id)
        {
            Slider slider = _sql.Sliders.Find(id);
            if (slider == null) return NotFound();
            Helper.DeleteImg(_env.WebRootPath, "assets/images/product",slider.Image);
            _sql.Sliders.Remove(slider);
            _sql.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
