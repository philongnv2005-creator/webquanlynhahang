using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webquanlynhahang.Data;
using webquanlynhahang.Helpers;
using webquanlynhahang.Models;

namespace webquanlynhahang.Controllers;

[Authorize(Roles = RoleNames.NoiBo)]
public class KhachHangController : Controller
{
    private readonly AppDbContext _db;

    public KhachHangController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.KhachHangs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x => x.HoTen.Contains(q) || x.SoDienThoai.Contains(q));
        }
        ViewBag.Query = q;
        return View(await query.OrderBy(x => x.HoTen).ToListAsync());
    }

    [HttpGet]
    public IActionResult Create() => View(new KhachHang());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhachHang model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Thêm khách hàng thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.KhachHangs.FindAsync(id);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, KhachHang model)
    {
        if (id != model.MaKhachHang) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật khách hàng thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _db.KhachHangs
            .Include(x => x.DatBans).ThenInclude(x => x.BanAn)
            .Include(x => x.DonGoiMons).ThenInclude(x => x.HoaDon)
            .SingleOrDefaultAsync(x => x.MaKhachHang == id);
        return customer is null ? NotFound() : View(customer);
    }
}
