using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlackLagoon.Controllers
{
    
    public class VillaController : Controller
    {

        private readonly ApplicationDbContext _db;

       public  VillaController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Villa> villas = _db.Villas.ToList();

            
            return View(villas);
        }
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Create(Villa newVilla)
        {
            if (newVilla.Name == newVilla.Description)
            {
                ModelState.AddModelError("Name", "Name and Description can´t match");
            }
            if (ModelState.IsValid)
            {
                newVilla.CreatedDate = DateTime.Now;
                _db.Villas.Add(newVilla);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            return View(newVilla);
        }
    }
}
