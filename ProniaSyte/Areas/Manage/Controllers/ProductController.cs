using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _sql;
        public ProductController(AppDbContext sql,IWebHostEnvironment env)
        {
            _sql = sql;
            _env = env;
        }
        [AllowAnonymous]
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
            ViewBag.Categories =await _sql.Categories.ToListAsync();
            if (!ModelState.IsValid) return View();
            if (_sql.Products.Any(p => p.Name == product.Name))
            {
                ModelState.AddModelError("Name", "Bu adda product var!");
                return View();
            }
            if (product.ImageFiles != null)
            {
                if (CheckImage(product.ImageFiles) != "")
                {
                    ModelState.AddModelError("ImageFiles", CheckImage(product.ImageFiles));
                    return View();
                }
                product.Images = new List<ProductImage>();
                foreach (var item in product.ImageFiles)
                {
                    ProductImage productImage = new ProductImage
                    {
                        Image= item.SaveImage(_env.WebRootPath, "assets/images/product"),
                        IsMain=false,
                        Product=product
                    };
                    product.Images.Add(productImage);
                }
                if (product.MainImage.Length / 1024/1024 >= 5)
                {
                    ModelState.AddModelError("MainImage", "Sekil max 5 mb ola biler!");
                    return View();
                }
                if (!product.MainImage.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("MainImage", "Sekil formati duzgun deyil!");
                    return View();
                }
                ProductImage mainImage = new ProductImage
                {
                    Image = product.MainImage.SaveImage(_env.WebRootPath, "assets/images/product"),
                    IsMain = true,
                    Product = product
                };
                product.Images.Add(mainImage);
            }
            else
            {
                return NotFound();
            }
            if (product.IsDelete == false) product.IsDelete = true;
            await _sql.Products.AddAsync(product);
            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            ViewBag.Categories = _sql.Categories.ToList();
            Product product = _sql.Products.Include(p => p.Category).Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Product product)
        {
            if (!ModelState.IsValid) return View();
            Product existProduct = await _sql.Products.FindAsync(id);
            if (existProduct == null) return NotFound();
            List<ProductImage> productImages = await _sql.ProductImages.Where(pi => pi.ProductId == id).ToListAsync();
            List<int> dltImg = new List<int>();
            foreach (var item in productImages)
            {
                
            }
            if (existProduct.ImageFiles != null)
            {
                if (CheckImage(product.ImageFiles) != "")
                {
                    ModelState.AddModelError("ImageFiles", CheckImage(existProduct.ImageFiles));
                    return View();
                }
                foreach (var item in product.ImageFiles)
                {
                    ProductImage productImage = new ProductImage
                    {
                        Image = item.SaveImage(_env.WebRootPath, "assets/images/product"),
                        IsMain =false,
                        Product = existProduct
                    };
                    product.Images.Add(productImage);
                    ProductImage mainImage = new ProductImage
                    {
                        Image = item.SaveImage(_env.WebRootPath, "assets/images/product"),
                        IsMain = true,
                        Product = existProduct
                    };
                    product.Images.Add(mainImage);
                }
            }
            if (product.IsDelete == false) existProduct.IsDelete = true;
            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.Price = product.Price;
            existProduct.Rating = product.Rating;
            existProduct.StockCount = product.StockCount;
            existProduct.CategoryId = product.CategoryId;
            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int? id,Product product)
        {
            if (id == null) return NotFound();
            product = _sql.Products.Find(id);
            if (product == null) return BadRequest();
            foreach (var item in product.Images)
            {
                 Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/product",item.Image);
            }
            _sql.Products.Remove(product);
            _sql.SaveChanges();
            return RedirectToAction("Index");
        }
        public string CheckImage(IFormFileCollection images)
        {
            foreach (var item in images)
            {
                if (!item.IsImage())
                {
                    return $"{item.FileName} adli file duzgun formatda deyil!";
                }
                if (!item.IsSizeOk(5))
                {
                    return $"{item.FileName} adli file max 5 mb ola biler!";
                }
            }
            return "";
        }
    }
}
