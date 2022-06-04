using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaSyte.DAL;
using ProniaSyte.Extentions;
using ProniaSyte.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProniaSyte.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _sql;
        public ProductController(AppDbContext sql,IWebHostEnvironment env)
        {
            _sql = sql;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _sql.Products.Include(p => p.Category).Include(i => i.Images).ToListAsync();
            return View(products);
        }
        public IActionResult Create()
        {
            //ProductCategoryVM prct = new ProductCategoryVM();
            //prct.Categories = _sql.Categories.ToList();
            //return View(prct);
            ViewBag.Categories = _sql.Categories.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) {
                ViewBag.Categories = _sql.Categories.ToList();
                return View();
            };
            if (_sql.Products.Any(p => p.Name == product.Name))
            {
                ViewBag.Categories = _sql.Categories.ToList();
                ModelState.AddModelError("Name","Bu adda mehsul var!");
                return View();
            }
            if (product.ImageFile != null)
            {
                if (!product.ImageFile.IsImage())
                {
                    ModelState.AddModelError("Poster","Sekil duzgun formatda deyil!");
                    return View();
                }
                if (!product.ImageFile.IsSizeOk(10))
                {
                    ModelState.AddModelError("Poster", "Sekil duzgun formatda deyil!");
                    return View();
                }
                //foreach (var item in product.ImageFiles)
                //{
                //    ProductImage productImage = new ProductImage
                //    {
                //        Image = await item.SavaImage(_env.WebRootPath, "assets/images/product", product.Images),
                //        IsMain = true
                //    };
                   
                //}
            }
            await _sql.Products.AddAsync(product);
            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            ViewBag.Categories = _sql.Categories.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid) return View();
            Product existProduct = _sql.Products.Find(product.Id);
            if (existProduct == null) return NotFound();
            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.Price = product.Price;
            existProduct.Rating = product.Rating;
            existProduct.IsDelete = product.IsDelete;
            existProduct.StockCount = product.StockCount;
            existProduct.CategoryId = product.CategoryId;
            _sql.SaveChanges();
            return RedirectToAction("Index");
        }
       
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            Product product = _sql.Products.Find(id);
            if (product == null) return BadRequest();
            foreach (var item in product.Images)
            {
                 Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/product",item.Image);
            }
            _sql.Products.Remove(product);
            _sql.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
