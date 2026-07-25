using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Data;
using RestaurantManager.Helpers;
using RestaurantManager.Models;

namespace RestaurantManager.Controllers;

[Authorize(Roles = RoleNames.QuanTriHoacQuanLy)]
public class DanhMucController : Controller
{
    private readonly AppDbContext _db;

    public DanhMucController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.DanhMucMonAns.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x => x.TenDanhMuc.Contains(q));
        }
        ViewBag.Query = q;
        return View(await query.OrderBy(x => x.TenDanhMuc).ToListAsync());
    }

    [HttpGet]
    public IActionResult Create() => View(new DanhMucMonAn());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DanhMucMonAn model)
    {
        if (await _db.DanhMucMonAns.AnyAsync(x => x.TenDanhMuc == model.TenDanhMuc))
        {
            ModelState.AddModelError(nameof(model.TenDanhMuc), "Tên danh mục đã tồn tại.");
        }
        if (!ModelState.IsValid) return View(model);
        _db.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Thêm danh mục thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _db.DanhMucMonAns.FindAsync(id);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DanhMucMonAn model)
    {
        if (id != model.MaDanhMuc) return BadRequest();
        if (await _db.DanhMucMonAns.AnyAsync(x => x.TenDanhMuc == model.TenDanhMuc && x.MaDanhMuc != id))
        {
            ModelState.AddModelError(nameof(model.TenDanhMuc), "Tên danh mục đã tồn tại.");
        }
        if (!ModelState.IsValid) return View(model);
        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật danh mục thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var entity = await _db.DanhMucMonAns.FindAsync(id);
        if (entity is null) return NotFound();
        entity.TrangThai = !entity.TrangThai;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
