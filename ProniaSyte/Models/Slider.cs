using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProniaSyte.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public int DiscountPercent { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
