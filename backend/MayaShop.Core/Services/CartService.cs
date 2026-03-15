using MayaShop.Core.DTOs;
using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces;

namespace MayaShop.Core.Services;

public interface ICartService
{
    Task<CartResponseDto> GetCartByUserIdAsync(int userId);
    Task AddItemToCartAsync(int userId, AddToCartRequestDto dto);
    Task UpdateItemQuantityAsync(int userId, int cartItemId, UpdateCartItemQuantityDto dto);
    Task RemoveItemFromCartAsync(int userId, int cartItemId);
    Task ClearCartAsync(int userId);
}

public class CartService : ICartService
{
    private readonly IRepository<Cart> _cartRepo;
    private readonly IRepository<CartItem> _cartItemRepo;
    private readonly IRepository<Product> _productRepo;

    public CartService(IRepository<Cart> cartRepo, IRepository<CartItem> cartItemRepo, IRepository<Product> productRepo)
    {
        _cartRepo = cartRepo;
        _cartItemRepo = cartItemRepo;
        _productRepo = productRepo;
    }

    private async Task<Cart> GetOrCreateUserCartAsync(int userId)
    {
        var cart = await _cartRepo.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepo.AddAsync(cart);
            await _cartRepo.SaveChangesAsync();
        }
        return cart;
    }

    public async Task<CartResponseDto> GetCartByUserIdAsync(int userId)
    {
        var cart = await GetOrCreateUserCartAsync(userId);
        
        // Normalmente EF Core haría el trabajo pesado si estuviera configurado correctamente el query genérico
        var items = await _cartItemRepo.FindAsync(ci => ci.CartId == cart.Id);
        
        var dtoItems = new List<CartItemDto>();
        decimal total = 0;

        foreach (var item in items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                var price = product.Price;
                var subtotal = price * item.Quantity;
                total += subtotal;

                dtoItems.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = price,
                    SubTotal = subtotal
                });
            }
        }

        return new CartResponseDto
        {
            Id = cart.Id,
            Items = dtoItems,
            Total = total
        };
    }

    public async Task AddItemToCartAsync(int userId, AddToCartRequestDto dto)
    {
        var cart = await GetOrCreateUserCartAsync(userId);
        var product = await _productRepo.GetByIdAsync(dto.ProductId);
        
        if (product == null || !product.IsActive)
            throw new Exception("El producto no existe o no está disponible.");

        if (product.Stock < dto.Quantity)
            throw new Exception($"Stock insuficiente. Hay {product.Stock} disponibles.");

        // Buscar si ya lo tiene en el carrito
        var existingItem = await _cartItemRepo.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            if (existingItem.Quantity > product.Stock)
                throw new Exception($"Stock insuficiente al intentar añadir esta cantidad.");
            
            await _cartItemRepo.UpdateAsync(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            await _cartItemRepo.AddAsync(newItem);
        }

        await _cartItemRepo.SaveChangesAsync();
    }

    public async Task UpdateItemQuantityAsync(int userId, int cartItemId, UpdateCartItemQuantityDto dto)
    {
        var cart = await GetOrCreateUserCartAsync(userId);
        // Validamos que el ítem le pertenezca a su carrito
        var item = await _cartItemRepo.FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cart.Id);

        if (item == null) throw new KeyNotFoundException("Ítem no encontrado en el carrito.");

        var product = await _productRepo.GetByIdAsync(item.ProductId);
        if (product == null || product.Stock < dto.Quantity)
             throw new Exception("No hay suficiente stock para esa cantidad.");

        if (dto.Quantity <= 0)
        {
            await _cartItemRepo.DeleteAsync(item);
        }
        else
        {
            item.Quantity = dto.Quantity;
            await _cartItemRepo.UpdateAsync(item);
        }

        await _cartItemRepo.SaveChangesAsync();
    }

    public async Task RemoveItemFromCartAsync(int userId, int cartItemId)
    {
        var cart = await GetOrCreateUserCartAsync(userId);
        var item = await _cartItemRepo.FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cart.Id);

        if (item != null)
        {
            await _cartItemRepo.DeleteAsync(item);
            await _cartItemRepo.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(int userId)
    {
        var cart = await GetOrCreateUserCartAsync(userId);
        var items = await _cartItemRepo.FindAsync(ci => ci.CartId == cart.Id);
        
        if (items.Any())
        {
            await _cartItemRepo.DeleteRangeAsync(items);
            await _cartItemRepo.SaveChangesAsync();
        }
    }
}
