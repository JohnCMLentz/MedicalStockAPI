using MedicalStock.Api.Data;
using MedicalStock.Api.DTOs.Products;
using MedicalStock.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalStock.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .Select(product => new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductResponse?> GetByBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken)
    {
        string normalizedBarcode = barcode.Trim();

        return await _context.Products
            .AsNoTracking()
            .Where(product => product.Barcode == normalizedBarcode)
            .Select(product => new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        string normalizedName = request.Name.Trim();
        string normalizedBarcode = request.Barcode.Trim();
        string normalizedManufacturer = request.Manufacturer.Trim();

        ValidateProductData(
            normalizedName,
            normalizedBarcode,
            normalizedManufacturer,
            request.Price);

        bool barcodeAlreadyExists = await _context.Products
            .AnyAsync(
                product => product.Barcode == normalizedBarcode,
                cancellationToken);

        if (barcodeAlreadyExists)
        {
            throw new InvalidOperationException(
                "A product with this barcode already exists.");
        }

        Category? category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (category is null)
        {
            throw new KeyNotFoundException(
                "The informed category was not found.");
        }

        var product = new Product
        {
            Name = normalizedName,
            Barcode = normalizedBarcode,
            Manufacturer = normalizedManufacturer,
            Price = request.Price,
            CategoryId = request.CategoryId
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);

        return ToResponse(product, category.Name);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        Product? product = await _context.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);

        if (product is null)
            return false;

        string normalizedName = request.Name.Trim();
        string normalizedBarcode = request.Barcode.Trim();
        string normalizedManufacturer = request.Manufacturer.Trim();

        ValidateProductData(
            normalizedName,
            normalizedBarcode,
            normalizedManufacturer,
            request.Price);

        bool barcodeAlreadyExists = await _context.Products
            .AnyAsync(
                existingProduct =>
                    existingProduct.Id != id &&
                    existingProduct.Barcode == normalizedBarcode,
                cancellationToken);

        if (barcodeAlreadyExists)
        {
            throw new InvalidOperationException(
                "A product with this barcode already exists.");
        }

        bool categoryExists = await _context.Categories
            .AnyAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            throw new KeyNotFoundException(
                "The informed category was not found.");
        }

        product.Name = normalizedName;
        product.Barcode = normalizedBarcode;
        product.Manufacturer = normalizedManufacturer;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        Product? product = await _context.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);

        if (product is null)
            return false;

        _context.Products.Remove(product);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateProductData(
        string name,
        string barcode,
        string manufacturer,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");

        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Product barcode is required.");

        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new ArgumentException("Product manufacturer is required.");

        if (price <= 0)
            throw new ArgumentException(
                "Product price must be greater than zero.");
    }

    private static ProductResponse ToResponse(
        Product product,
        string categoryName)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            Manufacturer = product.Manufacturer,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = categoryName
        };
    }
}