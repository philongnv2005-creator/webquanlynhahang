using RestaurantManager.Models;

namespace RestaurantManager.Services;

public interface IAuthService
{
    Task<NguoiDung?> ValidateUserAsync(string username, string password);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    string HashPassword(NguoiDung user, string password);
}
