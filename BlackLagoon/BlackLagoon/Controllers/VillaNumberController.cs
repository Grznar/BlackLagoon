using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using BlackLagoon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            List<VillaNumber> villas = _db.VillaNumbers.Include(u=>u.Villa).ToList();

            
            return View(villas);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            
            
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Create(VillaNumberVM newVilla)
        {
            bool roomNumberExists = _db.VillaNumbers.Any(u => u.Villa_Number == newVilla.VillaNumber.Villa_Number);
            if (ModelState.IsValid && !roomNumberExists)
            {
                
                
                    _db.VillaNumbers.Add(newVilla.VillaNumber);
                    _db.SaveChanges();
                    TempData["success"] = "Villa created successfully";
                
                
                return RedirectToAction(nameof(Index));
            }
            if(roomNumberExists)
            {
                TempData["error"] = "Villa number already exists";
            }
            newVilla.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(newVilla);
        }
        
        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber=_db.VillaNumbers.FirstOrDefault(u=>u.Villa_Number == villaNumberId)
            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        public IActionResult Update(VillaNumberVM newVilla)
        {
           
            if (ModelState.IsValid)
            {


                _db.VillaNumbers.Update(newVilla.VillaNumber);
                _db.SaveChanges();
                TempData["success"] = "Villa updated successfully";


                return RedirectToAction(nameof(Index));
            }
            
            
               TempData["error"] = "Villa number already exists";
            
            newVilla.VillaList = _db.Villas.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(newVilla);
        }
        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _db.Villas.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == villaNumberId)
            };
            if (villaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }
            [HttpPost]
        public IActionResult Delete(VillaNumberVM obj)
        {
            VillaNumber? villaFromDb= _db.VillaNumbers.FirstOrDefault(u => u.Villa_Number == obj.VillaNumber.Villa_Number);
            if (villaFromDb != null)
            {
                _db.VillaNumbers.Remove(villaFromDb);
                _db.SaveChanges();
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(Index));
            }


            TempData["error"] = "Not deleted!";

            
            return View(obj);

        }

    }
}
