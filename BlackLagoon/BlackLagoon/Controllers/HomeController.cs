using System.Diagnostics;
using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Models;
using BlackLagoon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlackLagoon.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        

        public IActionResult Index()
        {
            HomePageVM homeVM = new()
            {
                VillaList = _unitOfWork.Villas.GetAll(includeProperties:"VillaAmenity"),
                NumberOfNights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)

            };

            return View(homeVM);
        }
        [HttpPost]
        public IActionResult GetVillasByDate(int nights,DateOnly checkInDate)
        {
            var list = _unitOfWork.Villas.GetAll(includeProperties: "VillaAmenity");
            foreach (var villa in list)
            {
                if (villa.Id % 2 == 0)
                {
                    villa.IsAvailible = false;
                }
            }
            HomePageVM homeVM = new()
            {
                CheckInDate = checkInDate,
                VillaList = list,
                NumberOfNights = nights
            };
            return PartialView("_VillaList",homeVM);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
