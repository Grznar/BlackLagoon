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
