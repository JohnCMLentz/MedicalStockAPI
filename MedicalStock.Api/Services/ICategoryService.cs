using MedicalStock.Api.DTOs.Categories;

namespace MedicalStock.Api.Services
{
    public interface ICategoryService
    {

        Task<IReadOnlyList<CategoryResponse>>GetAllAsync(CancellationToken cancellationToken);

        Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
