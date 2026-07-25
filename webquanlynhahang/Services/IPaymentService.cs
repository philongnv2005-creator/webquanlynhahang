using webquanlynhahang.Models.Enums;

namespace webquanlynhahang.Services;

public interface IPaymentService
{
    Task<int> PayAsync(int orderId, int cashierId, decimal discount, PhuongThucThanhToan method);
}
