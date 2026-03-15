namespace MayaShop.Core.DTOs;

public class CartResponseDto
{
    public int Id { get; set; }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal Total { get; set; }
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}

public class AddToCartRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemQuantityDto
{
    public int Quantity { get; set; }
}
