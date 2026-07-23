using RestaurantManager.Models.Enums;

namespace RestaurantManager.Services;

public interface IPaymentService
{
    Task<int> PayAsync(int orderId, int cashierId, decimal discount, PhuongThucThanhToan method);
}
