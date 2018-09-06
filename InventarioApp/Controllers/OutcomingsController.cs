using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioApp.Models;
using Rotativa.AspNetCore;
using System.Globalization;

namespace InventarioApp.Controllers
{
    public class OutcomingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OutcomingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Outcomings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OutcomingProducts.Include(o => o.Porduct).OrderByDescending(o => o.OutcomingDate);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ReportPdf()
        {
            var salidas = await _context.OutcomingProducts.Include(o => o.Porduct).OrderByDescending(o => o.OutcomingDate).ToListAsync();
            return new ViewAsPdf("ReportPDF", salidas);
        }

        // GET: Outcomings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outcomingProduct = await _context.OutcomingProducts
                .Include(o => o.Porduct)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (outcomingProduct == null)
            {
                return NotFound();
            }

            return View(outcomingProduct);
        }

        // GET: Outcomings/Create
        public IActionResult Create(int? id)
        {
            var products = _context.Products.Where(p => p.IsRemoved == false && (id == null || p.Id == id)).ToList();
            ViewData["ProductId"] = new SelectList(products, "Id", "Fullname");
            return View();
        }

        // POST: Outcomings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(OutcomingProduct[] outcomingProducts, string person)
        {
            var date = DateTime.Now;

            var recibo = new Receipt();
            recibo.Date = date;
            recibo.Beneficiary = person;
            _context.Receipts.Add(recibo);
            await _context.SaveChangesAsync();

            foreach (var outcomingProduct in outcomingProducts)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == outcomingProduct.ProductId);

                if (product.Total >= outcomingProduct.Quantity)
                {
                    product.Total -= outcomingProduct.Quantity;
                    outcomingProduct.OutcomingDate = date;

                    _context.Add(outcomingProduct);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    
                    var entrega = new Delivery();
                    entrega.OutcommingId = outcomingProduct.Id;
                    entrega.ProductId = product.Id;
                    entrega.ReceiptId = recibo.Id;
                    entrega.Cantidad = outcomingProduct.Quantity;
                    _context.Deliveries.Add(entrega);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    ViewBag.lessExistences = "*El producto solo cuenta con " + product.Total + " unidades";
                }
            }
            
            return Ok(recibo.Id);

        }

        [HttpGet]
        public IActionResult OutcomingsPdf(int id)
        {
            CultureInfo culture = new CultureInfo("es-ES");

            var recibo = _context.Receipts.FirstOrDefault(r => r.Id == id);

            ViewBag.fechaRecibo = recibo.Date.ToString(culture.DateTimeFormat.LongDatePattern, culture);
            var viewModel = new ReceiptsViewModel();

            viewModel.Recibo = recibo;

            if(recibo != null)
            {
                var productosId = _context.Deliveries.Where(d => d.ReceiptId == id).ToList();
                viewModel.productos = new List<Product>();
                foreach (var item in productosId)
                {
                    var producto = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);

                    if(producto != null )
                    {
                        producto.Total = item.Cantidad;

                        viewModel.productos.Add(producto);
                    }

                }
                viewModel.Fecha = recibo.Date.ToString(culture.DateTimeFormat.LongDatePattern, culture);
            }
            


            return new ViewAsPdf("EntregaPDF", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CheckProductExistence(int productId, int qty = 0)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product != null)
            {
                if (product.Total >= qty)
                {
                    return Ok();
                }
                else
                {
                    return NoContent();
                }
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Outcomings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outcomingProduct = await _context.OutcomingProducts.SingleOrDefaultAsync(m => m.Id == id);
            if (outcomingProduct == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", outcomingProduct.ProductId);
            return View(outcomingProduct);
        }

        // POST: Outcomings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity,OutcomingDate")] OutcomingProduct outcomingProduct)
        {
            if (id != outcomingProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(outcomingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OutcomingProductExists(outcomingProduct.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Code", outcomingProduct.ProductId);
            return View(outcomingProduct);
        }

        // GET: Outcomings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outcomingProduct = await _context.OutcomingProducts
                .Include(o => o.Porduct)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (outcomingProduct == null)
            {
                return NotFound();
            }

            return View(outcomingProduct);
        }

        // POST: Outcomings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var outcomingProduct = await _context.OutcomingProducts.SingleOrDefaultAsync(m => m.Id == id);
            _context.OutcomingProducts.Remove(outcomingProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OutcomingProductExists(int id)
        {
            return _context.OutcomingProducts.Any(e => e.Id == id);
        }
    }
}
