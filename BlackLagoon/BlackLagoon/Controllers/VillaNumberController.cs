using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlackLagoon.Controllers
{
    
    public class VillaNumberController : Controller
    {

        private readonly ApplicationDbContext _db;

       public VillaNumberController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<VillaNumber> villas = _db.VillaNumbers.ToList();

            
            return View(villas);
        }
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> list = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ViewData["VillaList"] = list;
            ViewBag.VillaList = list;
            return View();
        }
        [HttpPost]
        public IActionResult Create(VillaNumber newVilla)
        {
            
            if (ModelState.IsValid)
            {
                
                _db.VillaNumbers.Add(newVilla);
                _db.SaveChanges();
                TempData["success"] = "Villa created successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            return View(newVilla);
        }
        
        public IActionResult Update(int villaId)
        {
            Villa? villa = _db.Villas.FirstOrDefault(u=>u.Id==villaId);
            if(villa == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(villa);
        }
        [HttpPost]
        public IActionResult Update(Villa newVilla)
        {
            if(ModelState.IsValid)
            {
                _db.Villas.Update(newVilla);
                _db.SaveChanges();
                TempData["success"] = "Villa updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {

                TempData["error"] = "Villa not updated";
                return View(newVilla);
            }
        }
        public IActionResult Delete(int villaId)
        {
            Villa? villa = _db.Villas.FirstOrDefault(u => u.Id == villaId);
            if (villa is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villa);
        }
        [HttpPost]
        public IActionResult Delete(Villa newVilla)
        {
            Villa? villaFromDb= _db.Villas.FirstOrDefault(u => u.Id == newVilla.Id);
            if (villaFromDb is not null)
            {
                _db.Villas.Remove(villaFromDb);
                _db.SaveChanges();
                TempData["success"] = "Villa deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                
                TempData["error"] = "Villa not deleted";
                return View();
            }
            
        }

    }
}
