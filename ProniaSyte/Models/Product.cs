using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProniaSyte.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public string Description { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public int StockCount { get; set; }
        public bool IsDelete { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public List<ProductImage> Images { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }
    }
}
