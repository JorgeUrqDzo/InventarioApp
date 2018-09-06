using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioApp.Models;
using Rotativa.AspNetCore;

namespace InventarioApp.Controllers
{
    public class IncomingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Incomings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.IncomingProducts.Include(i => i.Porduct).OrderByDescending(i => i.IncomingDate);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ReportPdf()
        {
            var applicationDbContext = _context.IncomingProducts.Include(i => i.Porduct).OrderByDescending(i => i.IncomingDate);
            return new ViewAsPdf("ReportPDF", await applicationDbContext.ToListAsync());
        }

        // GET: Incomings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomingProduct = await _context.IncomingProducts
                .Include(i => i.Porduct)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (incomingProduct == null)
            {
                return NotFound();
            }

            return View(incomingProduct);
        }

        // GET: Incomings/Create
        public IActionResult Create(int? id, int? donorId)
        {
            var donors = _context.Donors.Where(d => (donorId == null || d.Id == donorId)).ToList();

            ViewData["DonorId"] = new SelectList(donors, "Id", "Fullname");

            var products = _context.Products.Where(p => p.IsRemoved == false && (id == null || p.Id == id)).ToList();

            ViewData["ProductId"] = new SelectList(products, "Id", "Fullname");

            return View();
        }

        // POST: Incomings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Quantity,IncomingDate")] IncomingProduct incomingProduct, int donorId)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == incomingProduct.ProductId);
                product.Total += incomingProduct.Quantity;

                var date = DateTime.Now;
                incomingProduct.IncomingDate = date;

                _context.Add(incomingProduct);
                _context.Update(product);

                await _context.SaveChangesAsync();

                var donation = new Donations();
                donation.DonorId = donorId;
                donation.ProductId = incomingProduct.ProductId;
                donation.Quantity = incomingProduct.Quantity;
                donation.DonationDate = date;

                _context.Add(donation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", incomingProduct.ProductId);
            return RedirectToAction("index", "Products");
            //return View(incomingProduct);
        }

        // GET: Incomings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomingProduct = await _context.IncomingProducts.SingleOrDefaultAsync(m => m.Id == id);
            if (incomingProduct == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", incomingProduct.ProductId);
            return View(incomingProduct);
        }

        // POST: Incomings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity,IncomingDate")] IncomingProduct incomingProduct)
        {
            if (id != incomingProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incomingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncomingProductExists(incomingProduct.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", incomingProduct.ProductId);
            return View(incomingProduct);
        }

        // GET: Incomings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomingProduct = await _context.IncomingProducts
                .Include(i => i.Porduct)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (incomingProduct == null)
            {
                return NotFound();
            }

            return View(incomingProduct);
        }

        // POST: Incomings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incomingProduct = await _context.IncomingProducts.SingleOrDefaultAsync(m => m.Id == id);
            _context.IncomingProducts.Remove(incomingProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncomingProductExists(int id)
        {
            return _context.IncomingProducts.Any(e => e.Id == id);
        }
    }
}
