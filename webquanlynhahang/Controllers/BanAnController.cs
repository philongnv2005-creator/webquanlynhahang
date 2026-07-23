using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Helpers;
using RestaurantManager.Models;

namespace RestaurantManager.Controllers;

[Authorize(Roles = RoleNames.NoiBo)]
public class BanAnController : Controller
{
    private readonly AppDbContext _db;

    public BanAnController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.BanAns.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x => x.TenBan.Contains(q) || x.KhuVuc.Contains(q));
        }
        ViewBag.Query = q;
        return View(await query.OrderBy(x => x.KhuVuc).ThenBy(x => x.TenBan).ToListAsync());
    }

    [Authorize(Roles = RoleNames.QuanTriHoacQuanLy)]
    [HttpGet]
    public IActionResult Create() => View(new BanAn());

    [Authorize(Roles = RoleNames.QuanTriHoacQuanLy)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BanAn model)
    {
        if (await _db.BanAns.AnyAsync(x => x.TenBan == model.TenBan))
            ModelState.AddModelError(nameof(model.TenBan), "Tên bàn đã tồn tại.");
        if (!ModelState.IsValid) return View(model);
        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Thêm bàn ăn thành công.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RoleNames.QuanTriHoacQuanLy)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.BanAns.FindAsync(id);
        return entity is null ? NotFound() : View(entity);
    }

    [Authorize(Roles = RoleNames.QuanTriHoacQuanLy)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BanAn model)
    {
        if (id != model.MaBan) return BadRequest();
        if (await _db.BanAns.AnyAsync(x => x.TenBan == model.TenBan && x.MaBan != id))
            ModelState.AddModelError(nameof(model.TenBan), "Tên bàn đã tồn tại.");
        if (!ModelState.IsValid) return View(model);
        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật bàn ăn thành công.";
        return RedirectToAction(nameof(Index));
    }
}
