using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using BlackLagoon.Infrastructure.Repository;
using BlackLagoon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlackLagoon.Controllers
{
    
    public class AmenityController : Controller
    {

        public IUnitOfWork _unitOfwork;

        public AmenityController(IUnitOfWork unitOfwork)
        {
            _unitOfwork = unitOfwork;
        }
        public IActionResult Index()
        {
            List<Amenity> villas = _unitOfwork.Amenities.GetAll(includeProperties: "Villa").ToList();

            
            return View(villas);
        }
        public IActionResult Create()
        {
            AmenityVM AmenityVM = new AmenityVM()
            {
                VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            
            
            return View(AmenityVM);
        }
        [HttpPost]
        public IActionResult Create(AmenityVM newVilla)
        {
           
            if (ModelState.IsValid )
            {


                _unitOfwork.Amenities.Add(newVilla.Amenity);
                _unitOfwork.Save();
                    TempData["success"] = "Amenity created successfully";
                
                
                return RedirectToAction(nameof(Index));
            }
            
            
                TempData["error"] = "Amenity number already exists";
            
            newVilla.VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(newVilla);
        }
        
        public IActionResult Update(int AmenityId)
        {
            AmenityVM AmenityVM = new AmenityVM()
            {
                VillaList = _unitOfwork.Villas.GetAll().ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfwork.Amenities.Get((u => u.Id == AmenityId))

            };
            if (AmenityVM.Amenity == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(AmenityVM);
        }
        [HttpPost]
        public IActionResult Update(AmenityVM newVilla)
        {
           
            if (ModelState.IsValid)
            {


                _unitOfwork.Amenities.Update(newVilla.Amenity);
                _unitOfwork.Save();
                TempData["success"] = "Amenity updated successfully";


                return RedirectToAction(nameof(Index));
            }
            
            
             
            
            newVilla.VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(newVilla);
        }
        public IActionResult Delete(int AmenityId)
        {
            AmenityVM AmenityVM = new AmenityVM()
            {
                VillaList = _unitOfwork.Villas.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfwork.Amenities.Get(u => u.Id == AmenityId)
            };
            if (AmenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(AmenityVM);
        }
            [HttpPost]
        public IActionResult Delete(AmenityVM obj)
        {
            Amenity? villaFromDb= _unitOfwork.Amenities.Get(u => u.Id == obj.Amenity.Id);
            if (villaFromDb != null)
            {
                _unitOfwork.Amenities.Delete(villaFromDb);
                _unitOfwork.Save();
                TempData["success"] = "Deleted successfully";
                return RedirectToAction(nameof(Index));
            }


            TempData["error"] = "Not deleted!";

            
            return View(obj);

        }

    }
}
