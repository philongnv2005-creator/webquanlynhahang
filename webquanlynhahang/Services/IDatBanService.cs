using RestaurantManager.Models;

namespace RestaurantManager.Services;

public interface IDatBanService
{
    Task<(bool IsValid, string? Error)> ValidateAsync(DatBan datBan, int? ignoreId = null);
    Task UpdateTableStatusAsync(int tableId);
}
