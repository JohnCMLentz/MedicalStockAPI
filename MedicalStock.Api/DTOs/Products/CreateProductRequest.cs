using System.ComponentModel.DataAnnotations;

namespace MedicalStock.Api.DTOs.Products
{
    public class CreateProductRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 1)]
        public string Barcode { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Manufacturer { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "99999999.99")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
