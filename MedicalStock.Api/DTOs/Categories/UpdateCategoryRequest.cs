using System.ComponentModel.DataAnnotations;

namespace MedicalStock.Api.DTOs.Categories
{
    public class UpdateCategoryRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}
