using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VentaZapatillas.Data;
using VentaZapatillas.Models;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace VentaZapatillas.Controllers
{
    public class ZapatillasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ZapatillasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Zapatillas
        public async Task<IActionResult> Index()
        {
            var zapatillas = await _context.Zapatillas.ToListAsync();
            return View(zapatillas);
        }

        // GET: Zapatillas/Create
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Zapatilla zapatilla, IFormFile ImagenSubida)
{
    if (ModelState.IsValid)
    {
        if (ImagenSubida != null && ImagenSubida.Length > 0)
        {
            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(ImagenSubida.FileName);
            var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/zapatillas", nombreArchivo);

            using (var stream = new FileStream(rutaGuardado, FileMode.Create))
            {
                await ImagenSubida.CopyToAsync(stream);
            }

            zapatilla.ImagenUrl = "/img/zapatillas/" + nombreArchivo;
        }

        _context.Add(zapatilla);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(zapatilla);
}

        // POST: Zapatillas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Zapatilla zapatilla)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zapatilla);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zapatilla);
        }

        // GET: Zapatillas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var zapatilla = await _context.Zapatillas.FindAsync(id);
            if (zapatilla == null)
                return NotFound();

            return View(zapatilla);
        }

        // POST: Zapatillas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, Zapatilla zapatilla)
        {
            if (id != zapatilla.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(zapatilla);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zapatilla);
        }

        // GET: Zapatillas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var zapatilla = await _context.Zapatillas.FindAsync(id);
            if (zapatilla == null)
                return NotFound();

            return View(zapatilla);
        }

        // POST: Zapatillas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zapatilla = await _context.Zapatillas.FindAsync(id);
            _context.Zapatillas.Remove(zapatilla);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
