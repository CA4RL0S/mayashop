using System.Security.Claims;
using MayaShop.Core.DTOs;
using MayaShop.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MayaShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IUserService _userService;

    public CartController(ICartService cartService, IUserService userService)
    {
        _cartService = cartService;
        _userService = userService;
    }

    // Helper para obtener el int UserId desde el Token de Azure (azureOid)
    private async Task<int> GetInternalUserIdAsync()
    {
        var oidClaim = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier") 
                       ?? User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (oidClaim == null) throw new UnauthorizedAccessException("El token no es válido.");

        var user = await _userService.GetUserByAzureOidAsync(oidClaim.Value);
        if (user == null) throw new UnauthorizedAccessException("Usuario no sincronizado. Inicie sesión de nuevo.");

        return user.Id;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> GetCart()
    {
        var userId = await GetInternalUserIdAsync();
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(AddToCartRequestDto dto)
    {
        var userId = await GetInternalUserIdAsync();
        await _cartService.AddItemToCartAsync(userId, dto);
        return Ok();
    }

    [HttpPut("{itemId}")]
    public async Task<IActionResult> UpdateItemQuantity(int itemId, UpdateCartItemQuantityDto dto)
    {
        var userId = await GetInternalUserIdAsync();
        await _cartService.UpdateItemQuantityAsync(userId, itemId, dto);
        return Ok();
    }

    [HttpDelete("{itemId}")]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        var userId = await GetInternalUserIdAsync();
        await _cartService.RemoveItemFromCartAsync(userId, itemId);
        return Ok();
    }
}
