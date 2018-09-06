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
    public class DonorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Donors
        public async Task<IActionResult> Index(string name = null)
        {
            ViewData["name"] = name;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
            }

            var donors = await _context.Donors
                .Where(
                        d =>
                        (string.IsNullOrWhiteSpace(name) || (d.Name.ToLower().Contains(name) || d.LastName.ToLower().Contains(name) || d.MotherLastName.ToLower().Contains(name)))
                      ).ToListAsync();

            return View(donors);
        }

        public IActionResult DonationsByDonor(int id)
        {
            return new ViewAsPdf("DonacionesDonador", GetDonationsByDonorId(id));
        }

        public IActionResult Donations()
        {

            var donadores = _context.Donors.ToList();
            var donacionesVMList = new List<DonacionesModelView>();

            foreach (var donador in donadores)
            {
                DonacionesModelView donacionesVM = new DonacionesModelView();
                donacionesVM.DonationsList = GetDonationsByDonorId(donador.Id);
                donacionesVMList.Add(donacionesVM);
            }

            return new ViewAsPdf("DonacionesPDF", donacionesVMList);
        }

        private List<DonacionesModelView> GetDonationsByDonorId(int id)
        {
            var donaciones = _context.Donations.Where(d => d.DonorId == id)
                .OrderByDescending(d => d.DonationDate).ToList();

            DonacionesModelView donacionesVM;
            var donacionesVMList = new List<DonacionesModelView>();

            foreach (var donacion in donaciones)
            {
                donacionesVM = new DonacionesModelView();
                donacionesVM.Date = donacion.DonationDate;
                donacionesVM.Qty = donacion.Quantity;

                var product = _context.Products.FirstOrDefault(p => p.Id == donacion.ProductId);

                donacionesVM.Product = product.Fullname;

                var donor = _context.Donors.FirstOrDefault(d => d.Id == donacion.DonorId);

                donacionesVM.DonorName = donor.Fullname;

                donacionesVMList.Add(donacionesVM);
            }

            return donacionesVMList;
        }

        // GET: Donors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors
                .SingleOrDefaultAsync(m => m.Id == id);
            if (donor == null)
            {
                return NotFound();
            }

            return View(donor);
        }

        // GET: Donors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Donors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,MotherLastName")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(donor);
        }

        // GET: Donors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors.SingleOrDefaultAsync(m => m.Id == id);
            if (donor == null)
            {
                return NotFound();
            }
            return View(donor);
        }

        // POST: Donors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,MotherLastName")] Donor donor)
        {
            if (id != donor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonorExists(donor.Id))
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
            return View(donor);
        }

        // GET: Donors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donor = await _context.Donors
                .SingleOrDefaultAsync(m => m.Id == id);
            if (donor == null)
            {
                return NotFound();
            }

            return View(donor);
        }

        // POST: Donors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donor = await _context.Donors.SingleOrDefaultAsync(m => m.Id == id);
            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonorExists(int id)
        {
            return _context.Donors.Any(e => e.Id == id);
        }
    }
}
