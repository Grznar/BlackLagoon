using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using BlackLagoon.Infrastructure.Repository;
using BlackLagoon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlackLagoon.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class VillaNumberController : Controller
    {

        public IUnitOfWork _unitOfwork;

        public VillaNumberController(IUnitOfWork unitOfwork)
        {
            _unitOfwork = unitOfwork;
        }
        public IActionResult Index()
        {
            List<VillaNumber> villas = _unitOfwork.VillaNumbers.GetAll(includeProperties: "Villa").ToList();

            
            return View(villas);
        }
        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new VillaNumberVM()
            {
                VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
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
            bool roomNumberExists = _unitOfwork.VillaNumbers.Any(u => u.Villa_Number == newVilla.VillaNumber.Villa_Number);
            if (ModelState.IsValid && !roomNumberExists)
            {


                _unitOfwork.VillaNumbers.Add(newVilla.VillaNumber);
                _unitOfwork.Save();
                    TempData["success"] = "Villa created successfully";
                
                
                return RedirectToAction(nameof(Index));
            }
            if(roomNumberExists)
            {
                TempData["error"] = "Villa number already exists";
            }
            newVilla.VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
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
                VillaList = _unitOfwork.Villas.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfwork.VillaNumbers.Get((u => u.Villa_Number == villaNumberId))

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


                _unitOfwork.VillaNumbers.Update(newVilla.VillaNumber);
                _unitOfwork.Save();
                TempData["success"] = "Villa updated successfully";


                return RedirectToAction(nameof(Index));
            }
            
            
               TempData["error"] = "Villa number already exists";
            
            newVilla.VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
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
                VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfwork.VillaNumbers.Get(u => u.Villa_Number == villaNumberId)
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
            VillaNumber? villaFromDb= _unitOfwork.VillaNumbers.Get(u => u.Villa_Number == obj.VillaNumber.Villa_Number);
            if (villaFromDb != null)
            {
                _unitOfwork.VillaNumbers.Delete(villaFromDb);
                _unitOfwork.Save();
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(Index));
            }


            TempData["error"] = "Not deleted!";

            
            return View(obj);

        }

    }
}
