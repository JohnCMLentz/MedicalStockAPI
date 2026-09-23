using MedicalStock.Api.Data;
using MedicalStock.Api.DTOs.Categories;
using MedicalStock.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalStock.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            string normalizedName = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new ArgumentException("Category name is required.");

            bool nameAlreadyExists = _context.Categories.Any(c => c.Name.ToLower() == normalizedName.ToLower());

            if (nameAlreadyExists)
                throw new InvalidOperationException("Category name already exists.");

            var category = new Category
            {
                Name = normalizedName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(category);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (category == null)
                return false;

            string normalizedName = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                throw new ArgumentException("Category name is required.");

            bool nameAlreadyExists = await _context.Categories.AnyAsync(c => c.Id != id && c.Name.ToLower() == normalizedName.ToLower());

            if (nameAlreadyExists)
                throw new InvalidOperationException("Category name already exists.");

            category.Name = normalizedName;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

            if (category is null)
                return false;

            bool hasProducts = await _context.Products
                .AnyAsync(product => product.CategoryId == id, cancellationToken);

            if (hasProducts)
            {
                throw new InvalidOperationException(
                    "The category cannot be deleted because it has registered products.");
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static CategoryResponse ToResponse(Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            };
        }

    }
}
