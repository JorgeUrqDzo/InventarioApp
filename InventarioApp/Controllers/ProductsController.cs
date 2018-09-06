using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace InventarioApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnviroment;
        private readonly Dictionary<string, string> _MimmeTypes = new Dictionary<string, string>()
        {
            { ".txt", "text/plain"},
                { ".pdf", "application/pdf"},
                { ".doc", "application/vnd.ms-word"},
                { ".docx", "application/vnd.ms-word"},
                { ".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                { ".png", "image/png"},
                { ".jpg", "image/jpeg"},
                { ".jpeg", "image/jpeg"},
                { ".gif", "image/gif"},
                { ".csv", "text/csv"}
            };

        public ProductsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnviroment = hostingEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(string code = null, string name = null)
        {
            ViewData["code"] = code;
            ViewData["name"] = name;
            var products = await _context.Products
                .Where(
                        p => p.IsRemoved == false &&
                        (string.IsNullOrWhiteSpace(code) || p.Code.Contains(code)) &&
                        (string.IsNullOrWhiteSpace(name) || p.Name.Contains(name))
                      )
                .ToListAsync();

            double granTotal = 0;

            foreach (var item in products)
            {
                granTotal += item.Total;

            }

            ViewData["granTotal"] = granTotal;

            return View(products);
        }

        public async Task<IActionResult> ProductsPdf()
        {
            var productsVm = new ProductsViewModel();
            double granTotal = 0;

            var products = await _context.Products
                 .Where(
                         p => p.IsRemoved == false
                       )
                 .ToListAsync();

            if (products.Count() > 0)
            {

                foreach (var item in products)
                {
                    granTotal += item.Total;
                }

                productsVm.products = products;
                productsVm.CantTotalProductos = granTotal;
            }
            else
            {
                productsVm.products = new List<Product>();
                productsVm.CantTotalProductos = 0;
            }

            return new ViewAsPdf("InventarioPDF", productsVm);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var donors = _context.Donors.ToList();

            if (donors.Count() == 0)
            {
                return RedirectToAction("Create", "Donors");
            }

            ViewData["DonorId"] = new SelectList(donors, "Id", "Fullname");

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Code,Total")] Product product, IFormFile file, int donorId)
        {
            if (ModelState.IsValid)
            {
                var existCode = await _context.Products.AnyAsync(p => p.Code == product.Code);

                if (!existCode)
                {
                    if (file != null)
                    {
                        var type = file.ContentType;
                        var extension = _MimmeTypes.FirstOrDefault(t => t.Value == type).Key;
                        var uniqueFileName = Guid.NewGuid() + extension;

                        var fileName = Path.Combine(_hostingEnviroment.WebRootPath, "images", Path.GetFileName(uniqueFileName));

                        product.Image = uniqueFileName;

                        using (var stream = new FileStream(fileName, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                    }
                    else
                    {
                        product.Image = "default.png";
                    }

                    _context.Add(product);
                    //await _context.SaveChangesAsync();
                    var date = DateTime.Now;

                    var incomming = new IncomingProduct();
                    incomming.ProductId = product.Id;
                    incomming.Quantity = product.Total;
                    incomming.IncomingDate = date;

                    _context.Add(incomming);

                    var donation = new Donations();
                    donation.DonorId = donorId;
                    donation.ProductId = product.Id;
                    donation.Quantity = product.Total;
                    donation.DonationDate = date;

                    _context.Add(donation);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.alreadyExistCode = "El código de producto " + product.Code + " ya existe, asigne otro diferente.";

                    return View(product);
                }
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Code, Total, Image")] Product product, IFormFile file, string currentImage)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existCode = await _context.Products.AnyAsync(p => p.Code == product.Code && p.Id != id);

                    if (!existCode)
                    {
                        if (string.IsNullOrWhiteSpace(product.Image))
                        {
                            if (file != null)
                            {
                                var type = file.ContentType;
                                var extension = _MimmeTypes.FirstOrDefault(t => t.Value == type).Key;
                                var uniqueFileName = Guid.NewGuid() + extension;

                                var fileName = Path.Combine(_hostingEnviroment.WebRootPath, "images", Path.GetFileName(uniqueFileName));

                                product.Image = uniqueFileName;

                                using (var stream = new FileStream(fileName, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                            }
                            else
                            {
                                product.Image = "default.png";
                            }
                        }

                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ViewBag.alreadyExistCode = "El código de producto " + product.Code + " ya existe, asigne otro diferente.";

                        return View(product);
                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            product.IsRemoved = true;
            _context.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
