using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Common.Interfaces;
using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace BlackLagoon.Controllers
{
    
    public class VillaController : Controller
    {

        private readonly IUnitOfWork _unitOfwork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IUnitOfWork unitOfwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfwork = unitOfwork;
            _webHostEnvironment = webHostEnvironment;
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
                if (newVilla.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(newVilla.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images/VillaImage");

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        newVilla.Image.CopyTo(fileStream);
                    }
                    newVilla.ImageUrl = @"/Images/VillaImage/" + fileName;
                }
                else
                {
                    newVilla.ImageUrl = "https://placehold.co/600x400";
                }
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
                if(newVilla.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(newVilla.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images/VillaImage");

                    if (!string.IsNullOrEmpty(newVilla.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, newVilla.ImageUrl.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                        using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        newVilla.Image.CopyTo(fileStream);
                    }
                    newVilla.ImageUrl = @"/Images/VillaImage/"+fileName;
                }
                
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
                if (villa.ImageUrl is not null)
                {
                   

                    if (!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    
                }
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
