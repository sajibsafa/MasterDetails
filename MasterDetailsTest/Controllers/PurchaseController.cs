using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using MasterDetailsTest.Models;

namespace MasterDetailsTest.Controllers;

public class PurchaseController : Controller
{
    private readonly MasterDBContext _context;

    public PurchaseController(MasterDBContext context)
    {
        _context = context;
    }

    // GET: Purchase
    public async Task<IActionResult> Index()
    {
        var appDbContext = _context.Purchases.Include(p => p.Supplier);
        return View(await appDbContext.ToListAsync());
    }

    // GET: Purchase/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Purchases == null)
        {
            return NotFound();
        }

        var purchase = await _context.Purchases
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (purchase == null)
        {
            return NotFound();
        }

        return View(purchase);
    }

    // GET: Purchase/Create
    public IActionResult Create()
    {
        ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "SupplierName");
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductName");
        ViewData["UnitId"] = new SelectList(_context.Units, "Id", "UnitName");
        Purchase purchase = new Purchase();
        purchase.PurchaseProducts.Add(new PurchaseProduct { Id=1 });
        return View(purchase);
    }

    // POST: Purchase/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( Purchase purchase, IFormFile pictureFile1)
    {
        if (!ModelState.IsValid && purchase.SupplierId == 0)
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "SupplierName");
            ViewData["ProductId"] = new SelectList(_context.Suppliers, "Id", "ProductName");
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "UnitName");
            return View(purchase);
        }

        if (pictureFile1 != null && pictureFile1.Length > 0)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ManualRequisitionAttach", pictureFile1.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                pictureFile1.CopyTo(stream);
            }
            purchase.Image = $"{pictureFile1.FileName}";
        }

        purchase.PurchaseProducts.RemoveAll(x => x.Quantity == 0);

        _context.Add(purchase);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Purchase/Edit/5
    public IActionResult Edit(int? id)
    {
        ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "SupplierName");
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductName");
        ViewData["UnitId"] = new SelectList(_context.Units, "Id", "UnitName");


        Purchase purchase = _context.Purchases.Where(x => x.Id == id)
            .Include(i => i.PurchaseProducts)
            .ThenInclude(i => i.Product)
            .FirstOrDefault();
        purchase.PurchaseProducts.ForEach(x => x.Amount = x.Quantity * x.PurchasePrice);

       

        return View(purchase);
    }

    // POST: Purchase/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Purchase purchase)
    {
        purchase.PurchaseProducts.RemoveAll(x => x.Quantity == 0);

        try
        {
            List<PurchaseProduct> purchaseItems = _context.PurchaseProducts.Where(x => x.PurchaseId == purchase.Id).ToList();
            _context.PurchaseProducts.RemoveRange(purchaseItems);
            await _context.SaveChangesAsync();

            _context.Attach(purchase);
            _context.Entry(purchase).State = EntityState.Modified;
            _context.PurchaseProducts.AddRange(purchase.PurchaseProducts);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: Purchase/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        Purchase purchase = _context.Purchases.Where(x => x.Id == id)
            .Include(i => i.PurchaseProducts)
            .ThenInclude(i => i.Product)
            .FirstOrDefault();
        purchase.PurchaseProducts.ForEach(x => x.Amount = x.Quantity * x.PurchasePrice);

        ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "SupplierName");
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductName");

        return View(purchase);
    }

    // POST: Purchase/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Purchase purchase)
    {
        purchase.PurchaseProducts.RemoveAll(a => a.Quantity == 0);

        try
        {
            List<PurchaseProduct> purchaseItems = _context.PurchaseProducts.Where(x => x.PurchaseId == purchase.Id).ToList();
            _context.PurchaseProducts.RemoveRange(purchaseItems);
            await _context.SaveChangesAsync();

            _context.Attach(purchase);
            _context.Entry(purchase).State = EntityState.Deleted;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    private bool PurchaseExists(int id)
    {
      return _context.Purchases.Any(e => e.Id == id);
    }

    //public PurchaseProduct Map(Supplier source)
    //{
    //    var destination = new PurchaseProduct();

    //    // Check if VehicleTypes is not null before accessing its properties
    //    if (source.Supplier != null)
    //    {
    //        destination.VehicleTypeName = source.VehicleTypes.VehicleTypeName;
    //    }
    //    else
    //    {
    //        destination.VehicleTypeName = "";
    //    }

    //    // Map other properties if needed

    //    return destination;
    //}
}
