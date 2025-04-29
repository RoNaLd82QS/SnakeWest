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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Zapatillas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Zapatilla zapatilla, IFormFile ImagenSubida)
        {
            if (ModelState.IsValid)
            {
                if (ImagenSubida != null && ImagenSubida.Length > 0)
                {
                    var extension = Path.GetExtension(ImagenSubida.FileName).ToLowerInvariant();
                    var pesoMaximo = 2 * 1024 * 1024; // 2MB

                    if ((extension == ".jpg" || extension == ".jpeg" || extension == ".png") && ImagenSubida.Length <= pesoMaximo)
                    {
                        var nombreArchivo = Guid.NewGuid().ToString() + extension;
                        var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/zapatillas", nombreArchivo);

                        using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                        {
                            await ImagenSubida.CopyToAsync(stream);
                        }

                        zapatilla.ImagenUrl = "/img/zapatillas/" + nombreArchivo;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "❌ Solo se permiten imágenes .jpg, .jpeg o .png de máximo 2MB.";
                        return View(zapatilla);
                    }
                }

                _context.Add(zapatilla);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "✅ Zapatilla creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "❌ Error al crear la zapatilla.";
            return View(zapatilla);
        }

        // GET: Zapatillas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "❌ ID no válido.";
                return RedirectToAction(nameof(Index));
            }

            var zapatilla = await _context.Zapatillas.FindAsync(id);
            if (zapatilla == null)
            {
                TempData["ErrorMessage"] = "❌ Zapatilla no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(zapatilla);
        }

        // POST: Zapatillas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, Zapatilla zapatilla)
        {
            if (id != zapatilla.Id)
            {
                TempData["ErrorMessage"] = "❌ ID no coincide.";
                return RedirectToAction(nameof(Index));
            }

            var zapatillaExistente = await _context.Zapatillas.FindAsync(id);
            if (zapatillaExistente == null)
            {
                TempData["ErrorMessage"] = "❌ Zapatilla no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    zapatillaExistente.Nombre = zapatilla.Nombre;
                    zapatillaExistente.Descripcion = zapatilla.Descripcion;
                    zapatillaExistente.Precio = zapatilla.Precio;
                    zapatillaExistente.Stock = zapatilla.Stock;
                    zapatillaExistente.Talla = zapatilla.Talla;
                    zapatillaExistente.Marca = zapatilla.Marca;

                    if (zapatilla.ImagenSubida != null && zapatilla.ImagenSubida.Length > 0)
                    {
                        var extension = Path.GetExtension(zapatilla.ImagenSubida.FileName).ToLowerInvariant();
                        var pesoMaximo = 2 * 1024 * 1024;

                        if ((extension == ".jpg" || extension == ".jpeg" || extension == ".png") && zapatilla.ImagenSubida.Length <= pesoMaximo)
                        {
                            if (!string.IsNullOrEmpty(zapatillaExistente.ImagenUrl))
                            {
                                var rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", zapatillaExistente.ImagenUrl.TrimStart('/'));
                                if (System.IO.File.Exists(rutaAnterior))
                                    System.IO.File.Delete(rutaAnterior);
                            }

                            var nombreArchivo = Guid.NewGuid().ToString() + extension;
                            var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/zapatillas", nombreArchivo);

                            using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                            {
                                await zapatilla.ImagenSubida.CopyToAsync(stream);
                            }

                            zapatillaExistente.ImagenUrl = "/img/zapatillas/" + nombreArchivo;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "❌ Imagen inválida o muy pesada.";
                            return View(zapatilla);
                        }
                    }

                    _context.Update(zapatillaExistente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Zapatilla editada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "❌ Error al editar la zapatilla.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(zapatilla);
        }

        // GET: Zapatillas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "❌ ID no válido.";
                return RedirectToAction(nameof(Index));
            }

            var zapatilla = await _context.Zapatillas.FindAsync(id);
            if (zapatilla == null)
            {
                TempData["ErrorMessage"] = "❌ Zapatilla no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(zapatilla);
        }

        // POST: Zapatillas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var zapatilla = await _context.Zapatillas.FindAsync(id);
                if (zapatilla != null)
                {
                    if (!string.IsNullOrEmpty(zapatilla.ImagenUrl))
                    {
                        var rutaImagen = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", zapatilla.ImagenUrl.TrimStart('/'));
                        if (System.IO.File.Exists(rutaImagen))
                            System.IO.File.Delete(rutaImagen);
                    }

                    _context.Zapatillas.Remove(zapatilla);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "✅ Zapatilla eliminada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "❌ No se encontró la zapatilla.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "❌ Error al eliminar la zapatilla.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
