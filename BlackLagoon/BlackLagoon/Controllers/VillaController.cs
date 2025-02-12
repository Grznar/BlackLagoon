using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Common.Interfaces;
using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlackLagoon.Controllers
{
    
    public class VillaController : Controller
    {

        private readonly IUnitOfWork _unitOfwork;

       public VillaController(IUnitOfWork unitOfwork)
        {
            _unitOfwork = unitOfwork;
        }
        public IActionResult Index()
        {
            List<Villa> villas = _unitOfwork.Villas.GetAll().ToList();


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
                _unitOfwork.Villas.Add(newVilla);
                _unitOfwork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            return View(newVilla);
        }
        
        public IActionResult Update(int villaId)
        {
            Villa? villa = _unitOfwork.Villas.Get(u => u.Id == villaId);
            
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
                _unitOfwork.Villas.Update(newVilla);
                _unitOfwork.Save();
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
            Villa? villa = _unitOfwork.Villas.Get(u => u.Id == villaId);
            if (villa is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villa);
        }
        [HttpPost]
        public IActionResult Delete(Villa newVilla)
        {
            Villa? villa = _unitOfwork.Villas.Get(u => u.Id == newVilla.Id);
            if (villa is not null)
            {
                _unitOfwork.Villas.Delete(villa);
                _unitOfwork.Save();
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
