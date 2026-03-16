using System.Security.Claims;
using MayaShop.Core.DTOs;
using MayaShop.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MayaShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public OrdersController(IOrderService orderService, IUserService userService)
    {
        _orderService = orderService;
        _userService = userService;
    }

    private async Task<int> GetInternalUserIdAsync()
    {
        var oidClaim = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier") 
                       ?? User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (oidClaim == null) throw new UnauthorizedAccessException("Token no válido");

        var user = await _userService.GetUserByAzureOidAsync(oidClaim.Value);
        if (user == null) throw new UnauthorizedAccessException("Debe iniciar sesión primero");

        return user.Id;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
    {
        var userId = await GetInternalUserIdAsync();
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CheckoutCart(OrderCreateRequestDto dto)
    {
        var userId = await GetInternalUserIdAsync();
        var order = await _orderService.CreateOrderFromCartAsync(userId);
        return Ok(order);
    }

    // --- Endpoints de Admin ---
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        await _orderService.UpdateOrderStatusAsync(id, status);
        return NoContent();
    }
}
