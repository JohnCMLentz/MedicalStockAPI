using MedicalStock.Api.DTOs.Products;
using MedicalStock.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
    CancellationToken cancellationToken)
    {
        IReadOnlyList<ProductResponse> products =
            await _productService.GetAllAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(
    int id,
    CancellationToken cancellationToken)
    {
        ProductResponse? product =
            await _productService.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ProductResponse>> GetByBarcode(
    string barcode,
    CancellationToken cancellationToken)
    {
        ProductResponse? product =
            await _productService.GetByBarcodeAsync(
                barcode,
                cancellationToken);

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            ProductResponse product = await _productService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            bool updateResult = await _productService.UpdateAsync(id, request, cancellationToken);
            if (!updateResult)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        bool deleteResult = await _productService.DeleteAsync(id, cancellationToken);
        if (!deleteResult)
            return NotFound();
        return NoContent();
    }

}
