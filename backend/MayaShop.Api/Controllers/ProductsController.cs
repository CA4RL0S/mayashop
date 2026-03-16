using MayaShop.Core.DTOs;
using MayaShop.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MayaShop.Api.Controllers;

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
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts([FromQuery] int? categoryId)
    {
        var products = categoryId.HasValue 
            ? await _productService.GetProductsByCategoryAsync(categoryId.Value)
            : await _productService.GetAllProductsAsync(includeInactive: false);

        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null || !product.IsActive) return NotFound();
        return Ok(product);
    }

    // --- Endpoints de Admin ---
    // NOTA: Para este prototipo, vamos a evaluar que el usuario tenga rol Admin.
    // Esto se puede hacer chequeando claims si estuvieran inyectados, 
    // pero por ahora dependemos del Authorization policy o del middleware personalizado luego.

    [HttpPost]
    [Authorize] // En un sistema real usaríamos roles de App Registration. Aquí luego lo filtraremos.
    public async Task<ActionResult<ProductResponseDto>> CreateProduct(ProductCreateRequestDto dto)
    {
        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateProduct(int id, ProductCreateRequestDto dto)
    {
        await _productService.UpdateProductAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
