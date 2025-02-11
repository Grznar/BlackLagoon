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
