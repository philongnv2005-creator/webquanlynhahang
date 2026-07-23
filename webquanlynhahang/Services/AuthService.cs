using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Models;

namespace RestaurantManager.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<NguoiDung> _hasher = new();

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<NguoiDung?> ValidateUserAsync(string username, string password)
    {
        var user = await _db.NguoiDungs
            .Include(x => x.VaiTro)
            .SingleOrDefaultAsync(x => x.TenDangNhap == username);

        if (user is null || !user.TrangThai)
        {
            return null;
        }

        var result = _hasher.VerifyHashedPassword(user, user.MatKhauHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _db.NguoiDungs.FindAsync(userId);
        if (user is null || !user.TrangThai)
        {
            return false;
        }

        var result = _hasher.VerifyHashedPassword(user, user.MatKhauHash, oldPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            return false;
        }

        user.MatKhauHash = _hasher.HashPassword(user, newPassword);
        await _db.SaveChangesAsync();
        return true;
    }

    public string HashPassword(NguoiDung user, string password)
        => _hasher.HashPassword(user, password);
}
