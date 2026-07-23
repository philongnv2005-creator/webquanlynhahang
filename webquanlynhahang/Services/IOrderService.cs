using RestaurantManager.Models.Enums;

namespace RestaurantManager.Services;

public interface IOrderService
{
    Task<int> CreateOrderAsync(int tableId, int? customerId, int employeeId);
    Task AddItemAsync(int orderId, int dishId, int quantity);
    Task UpdateItemAsync(int itemId, int quantity, TrangThaiMonTrongDon status);
    Task CancelItemAsync(int itemId);
    Task MarkAwaitingPaymentAsync(int orderId);
}
