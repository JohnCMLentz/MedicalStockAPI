using MedicalStock.Api.DTOs.Products;

namespace MedicalStock.Api.Services
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProductResponse?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken);
        Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    }
}
